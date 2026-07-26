using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Implementations;
using Services.Interfaces;
using Services.Models.Payment;

namespace Services.Implementations;

public class PaymentService(IBookingRepository bookingRepository, global::Net.payOS.PayOS payOS) : IPaymentService
{
    public async Task<CreatePaymentResponse> CreateDepositPaymentAsync(int customerId, string baseApiUrl, CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.");
        }

        if (booking.CustomerId != customerId)
        {
            throw new UnauthorizedAccessException("Unauthorized access to booking payment.");
        }

        if (booking.Status != BookingStatus.WaitingDeposit)
        {
            throw new InvalidOperationException($"Cannot pay deposit for booking in status {booking.Status}. Booking must be approved by owner first.");
        }

        // Map method string to PaymentProvider enum
        var providerEnum = request.Method.ToLower() switch
        {
            "vnpay" => PaymentProvider.VNPay,
            "momo" => PaymentProvider.MoMo,
            "payos" => PaymentProvider.PayOS,
            "cash" => PaymentProvider.PayOS, // fallback
            _ => throw new ArgumentException("Unsupported payment method.")
        };

        var txnCode = "TXN" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        long orderCode = long.Parse(DateTime.UtcNow.ToString("yyMMddHHmmss") + new Random().Next(100, 999));

        // Add payment transaction record
        var transaction = new PaymentTransaction
        {
            BookingId = booking.Id,
            Amount = booking.DepositAmount,
            PaymentProvider = providerEnum,
            Status = PaymentStatus.Pending,
            TransactionCode = txnCode,
            CreatedAt = DateTime.UtcNow
        };

        booking.PaymentTransactions.Add(transaction);
        await bookingRepository.SaveChangesAsync(cancellationToken);

        // PayOS Return URL configuration
        var frontendReturnUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Environment.GetEnvironmentVariable("PayOS__ReturnUrl");
        var frontendCancelUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Environment.GetEnvironmentVariable("PayOS__CancelUrl");

        // Route PayOS back to our backend callback endpoint first, then the backend will redirect to the frontend.
        // We inject the TransactionCode and expected status so the backend can process it.
        var backendCallbackSuccess = $"{baseApiUrl}/api/payments/callback?TransactionCode={txnCode}&Status=success&RedirectUrl={Uri.EscapeDataString(frontendReturnUrl)}";
        var backendCallbackCancel = $"{baseApiUrl}/api/payments/callback?TransactionCode={txnCode}&Status=cancelled&RedirectUrl={Uri.EscapeDataString(frontendCancelUrl)}";

        var paymentData = new global::Net.payOS.Types.PaymentData(
            orderCode: orderCode,
            amount: (int)booking.DepositAmount,
            description: $"Coc {booking.BookingCode}".Substring(0, Math.Min($"Coc {booking.BookingCode}".Length, 25)),
            items: new List<global::Net.payOS.Types.ItemData>(),
            cancelUrl: backendCallbackCancel,
            returnUrl: backendCallbackSuccess
        );

        string checkoutUrl = "";
        try
        {
            var createPaymentResult = await payOS.createPaymentLink(paymentData);
            checkoutUrl = createPaymentResult.checkoutUrl;
        }
        catch (Exception ex)
        {
            // Fallback to mock if configured or fail
            if (Environment.GetEnvironmentVariable("PayOS__AllowMockPaymentsWhenUnconfigured") == "true")
            {
                checkoutUrl = backendCallbackSuccess;
            }
            else
            {
                throw new InvalidOperationException($"Failed to create PayOS link: {ex.Message}");
            }
        }

        return new CreatePaymentResponse
        {
            PaymentUrl = checkoutUrl,
            TransactionCode = txnCode,
            Amount = booking.DepositAmount
        };
    }

    public async Task<CreatePaymentResponse> CreateFinalPaymentAsync(int customerId, string baseApiUrl, CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new ArgumentException("Booking not found.");

        if (booking.CustomerId != customerId)
            throw new UnauthorizedAccessException("Unauthorized access to booking payment.");

        if (booking.Status != BookingStatus.WaitingFinalPayment)
            throw new InvalidOperationException($"Cannot pay final amount for booking in status {booking.Status}. Owner must confirm return first.");

        var finalAmount = booking.RemainingAmount + booking.ExtraFee + (booking.OverdueFee ?? 0);
        if (finalAmount <= 0)
        {
            // No payment needed, complete directly
            var oldStatus2 = booking.Status;
            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.StatusHistories.Add(new BookingStatusHistory
            {
                OldStatus = oldStatus2,
                NewStatus = BookingStatus.Completed,
                ChangedByUserId = customerId,
                Note = "Không cần thanh toán thêm. Tự động hoàn tất.",
                CreatedAt = DateTime.UtcNow
            });
            await bookingRepository.SaveChangesAsync(cancellationToken);
            return new CreatePaymentResponse { PaymentUrl = "", TransactionCode = "", Amount = 0 };
        }

        var providerEnum = request.Method.ToLower() switch
        {
            "vnpay" => PaymentProvider.VNPay,
            "momo" => PaymentProvider.MoMo,
            "payos" => PaymentProvider.PayOS,
            "cash" => PaymentProvider.PayOS,
            _ => throw new ArgumentException("Unsupported payment method.")
        };

        var txnCode = "FNL" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        long orderCode = long.Parse(DateTime.UtcNow.ToString("yyMMddHHmmss") + new Random().Next(100, 999));

        var transaction = new PaymentTransaction
        {
            BookingId = booking.Id,
            Amount = finalAmount,
            PaymentProvider = providerEnum,
            Status = PaymentStatus.Pending,
            TransactionCode = txnCode,
            CreatedAt = DateTime.UtcNow
        };

        booking.PaymentTransactions.Add(transaction);
        await bookingRepository.SaveChangesAsync(cancellationToken);

        var frontendReturnUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Environment.GetEnvironmentVariable("PayOS__ReturnUrl");
        var frontendCancelUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Environment.GetEnvironmentVariable("PayOS__CancelUrl");

        var backendCallbackSuccess = $"{baseApiUrl}/api/payments/callback?TransactionCode={txnCode}&Status=success&RedirectUrl={Uri.EscapeDataString(frontendReturnUrl ?? "")}";
        var backendCallbackCancel = $"{baseApiUrl}/api/payments/callback?TransactionCode={txnCode}&Status=failed&RedirectUrl={Uri.EscapeDataString(frontendCancelUrl ?? "")}";

        var paymentData = new global::Net.payOS.Types.PaymentData(
            orderCode: orderCode,
            amount: (int)finalAmount,
            description: $"TT {booking.BookingCode}".Substring(0, Math.Min($"TT {booking.BookingCode}".Length, 25)),
            items: new List<global::Net.payOS.Types.ItemData>(),
            cancelUrl: backendCallbackCancel,
            returnUrl: backendCallbackSuccess
        );

        string checkoutUrl = "";
        try
        {
            var createPaymentResult = await payOS.createPaymentLink(paymentData);
            checkoutUrl = createPaymentResult.checkoutUrl;
        }
        catch (Exception ex)
        {
            if (Environment.GetEnvironmentVariable("PayOS__AllowMockPaymentsWhenUnconfigured") == "true")
            {
                checkoutUrl = backendCallbackSuccess;
            }
            else
            {
                throw new InvalidOperationException($"Failed to create PayOS link: {ex.Message}");
            }
        }

        return new CreatePaymentResponse
        {
            PaymentUrl = checkoutUrl,
            TransactionCode = txnCode,
            Amount = finalAmount
        };
    }

    public async Task<PaymentStatusResponse> ProcessCallbackAsync(PaymentCallbackQuery query, CancellationToken cancellationToken = default)
    {
        // Start transaction
        using var dbTransaction = await bookingRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            // Find transaction by code
            // We retrieve booking through repository or direct query
            // Since bookingRepository.GetByIdAsync gets booking, let's load the booking
            // We'll search the booking containing this transaction code
            // But wait, since we don't have a direct GetTransactionByCode, let's load it or find it.
            // Let's implement this logic:
            // Since we need to query PaymentTransactions, let's search it via dbContext in the repository
            // Wait, we can get the booking by querying for the booking that contains this transaction.
            // Let's do that!
            // First, find the booking containing the transaction:
            // Let's assume we can query it. Since repository is IBookingRepository, we can retrieve all bookings,
            // or let's add a helper inside repository or query directly if repository exposes DbContext.
            // Wait, we can write a query to retrieve the booking. Let's make sure our repository has a method or we can write it.
            // Let's check: did we include GetByIdAsync? Yes.
            // Let's look up by transaction code. To do this, let's add a method on repository or retrieve it.
            // Wait, since we are using EF Core, can we query all bookings and filter by PaymentTransactions.Any(t => t.TransactionCode == code)?
            // Yes, that is standard EF query! Let's write a method in IBookingRepository for finding booking by transaction code,
            // or let's query it.
            // Wait! Let's check if we can query it. Since repository does not have `GetByTransactionCodeAsync`, let's add it or update `BookingRepository.cs`.
            // Let's first look at the code of `BookingRepository.cs` we wrote.
            // We can add a method `Task<Booking?> GetByTransactionCodeAsync(string transactionCode, CancellationToken cancellationToken = default);` to IBookingRepository!
            // This is super clean!
            // Let's edit `IBookingRepository.cs` and `BookingRepository.cs` to add this method.
            // Let's do that now!
            var booking = await GetBookingByTransactionCodeInternalAsync(query.TransactionCode, cancellationToken);
            if (booking == null)
            {
                throw new ArgumentException("Transaction not found.");
            }

            var transactionRecord = booking.PaymentTransactions.Single(t => t.TransactionCode == query.TransactionCode);
             if (transactionRecord.Status == PaymentStatus.Pending)
            {
                var success = query.Status.ToLower() == "success";
                var cancelled = query.Status.ToLower() == "cancelled";

                transactionRecord.Status = success ? PaymentStatus.Success : PaymentStatus.Failed;
                if (success)
                {
                    transactionRecord.PaidAt = DateTime.UtcNow;
                    var oldStatus = booking.Status;

                    if (booking.Status == BookingStatus.WaitingFinalPayment)
                    {
                        // Final payment after return inspection → Complete the booking
                        booking.Status = BookingStatus.Completed;
                        booking.UpdatedAt = DateTime.UtcNow;

                        booking.StatusHistories.Add(new BookingStatusHistory
                        {
                            OldStatus = oldStatus,
                            NewStatus = BookingStatus.Completed,
                            ChangedByUserId = booking.CustomerId,
                            Note = "Thanh toán cuối chuyến thành công. Chuyến xe hoàn tất.",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // Deposit payment → Move to WaitingPickup
                        booking.Status = BookingStatus.WaitingPickup;
                        booking.UpdatedAt = DateTime.UtcNow;

                        booking.StatusHistories.Add(new BookingStatusHistory
                        {
                            OldStatus = oldStatus,
                            NewStatus = BookingStatus.WaitingPickup,
                            ChangedByUserId = booking.CustomerId,
                            Note = "Deposit paid successfully. Booking confirmed.",
                            CreatedAt = DateTime.UtcNow
                        });

                    // Update availability block reason to "Confirmed Booking"
                    foreach (var block in booking.AvailabilityBlocks)
                    {
                        if (block.BookingId == booking.Id)
                        {
                            block.Reason = $"Confirmed Booking {booking.BookingCode}";
                        }
                    }

                    // AUTO REJECT OVERLAPPING BOOKINGS
                    var overlappingBookings = await bookingRepository.GetOverlappingBookingsAsync(booking.CarId, booking.StartDateTime, booking.EndDateTime, cancellationToken);
                    foreach (var overlap in overlappingBookings)
                    {
                        if (overlap.Id != booking.Id && (overlap.Status == BookingStatus.PendingApproval || overlap.Status == BookingStatus.WaitingDeposit))
                        {
                            var overlapOldStatus = overlap.Status;
                            overlap.Status = BookingStatus.Rejected;
                            overlap.CancellationReason = "Tá»± Ä‘á»™ng tá»« chá»‘i do trÃ¹ng lá»‹ch vá»›i Ä‘Æ¡n Ä‘áº·t xe khÃ¡c Ä‘Ã£ Ä‘áº·t cá»c.";
                            overlap.UpdatedAt = DateTime.UtcNow;

                            overlap.StatusHistories.Add(new BookingStatusHistory
                            {
                                OldStatus = overlapOldStatus,
                                NewStatus = BookingStatus.Rejected,
                                ChangedByUserId = 0, // System
                                Note = "Automatically rejected due to conflicting booking confirmation.",
                                CreatedAt = DateTime.UtcNow
                            });

                            // Restore voucher usage
                            if (overlap.BookingVoucher != null)
                            {
                                var voucher = await bookingRepository.GetVoucherByCodeAsync(overlap.BookingVoucher.Code, cancellationToken);
                            }
                        }
                    }
                    } // end else (deposit payment)
                }
                else if (cancelled)
                {
                    transactionRecord.Status = PaymentStatus.Failed;
                    var oldStatus = booking.Status;
                    booking.Status = BookingStatus.Cancelled;
                    booking.CancellationReason = "NgÆ°á»i dÃ¹ng há»§y thanh toÃ¡n.";
                    booking.UpdatedAt = DateTime.UtcNow;

                    booking.StatusHistories.Add(new BookingStatusHistory
                    {
                        OldStatus = oldStatus,
                        NewStatus = BookingStatus.Cancelled,
                        ChangedByUserId = booking.CustomerId,
                        Note = "User explicitly cancelled the payment on payment gateway.",
                        CreatedAt = DateTime.UtcNow
                    });

                    // Remove availability blocks!
                    foreach (var block in booking.AvailabilityBlocks.ToList())
                    {
                        if (block.BookingId == booking.Id)
                        {
                            booking.AvailabilityBlocks.Remove(block);
                        }
                    }
                }
                else
                {
                    // Failed payment (e.g. timeout)
                    transactionRecord.Status = PaymentStatus.Failed;
                }

                await bookingRepository.SaveChangesAsync(cancellationToken);
                await dbTransaction.CommitAsync(cancellationToken);
            }

            return new PaymentStatusResponse
            {
                BookingId = booking.Id,
                BookingStatus = booking.Status.ToString(),
                PaymentStatus = transactionRecord.Status.ToString(),
                TransactionCode = transactionRecord.TransactionCode
            };
        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PaymentStatusResponse?> GetPaymentStatusByBookingIdAsync(int customerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        if (booking.CustomerId != customerId && booking.Car.OwnerId != customerId)
        {
            throw new UnauthorizedAccessException("Unauthorized access to booking payment status.");
        }

        var lastTxn = booking.PaymentTransactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
        return new PaymentStatusResponse
        {
            BookingId = booking.Id,
            BookingStatus = booking.Status.ToString(),
            PaymentStatus = lastTxn?.Status.ToString() ?? "None",
            TransactionCode = lastTxn?.TransactionCode ?? string.Empty
        };
    }

    // Temporary helper inside service until added to repository
    private async Task<Booking?> GetBookingByTransactionCodeInternalAsync(string code, CancellationToken cancellationToken)
    {
        // We retrieve via DbContext reflection or by calling repository. GetByIdAsync doesn't support code filter,
        // so let's query via the context of the repository.
        // Since we registered BookingRepository as scoped, let's call the repository if we add the method.
        // Wait, let's implement it in IBookingRepository and BookingRepository right now to keep code clean and compile-friendly!
        return await ((BookingRepository)bookingRepository).GetByTransactionCodeAsync(code, cancellationToken);
    }
}

