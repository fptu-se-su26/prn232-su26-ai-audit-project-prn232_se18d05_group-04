using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Implementations;
using Services.Interfaces;
using Services.Models.Payment;

namespace Services.Implementations;

public class PaymentService(IBookingRepository bookingRepository) : IPaymentService
{
    public async Task<CreatePaymentResponse> CreateDepositPaymentAsync(int customerId, CreatePaymentRequest request, CancellationToken cancellationToken = default)
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
            "cash" => PaymentProvider.VNPay, // fallback
            _ => throw new ArgumentException("Unsupported payment method. Use 'vnpay' or 'momo'.")
        };

        var txnCode = "TXN" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);

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

        // Simulated payment gateway URL
        // It points to our API callback endpoint which updates status and redirects to client return url
        var callbackUrl = $"https://localhost:7005/api/payments/callback?TransactionCode={txnCode}&Status=success&RedirectUrl={Uri.EscapeDataString(request.ReturnUrl)}";

        return new CreatePaymentResponse
        {
            PaymentUrl = callbackUrl,
            TransactionCode = txnCode,
            Amount = booking.DepositAmount
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
                transactionRecord.Status = success ? PaymentStatus.Success : PaymentStatus.Failed;
                if (success)
                {
                    transactionRecord.PaidAt = DateTime.UtcNow;
                    var oldStatus = booking.Status;
                    booking.Status = BookingStatus.WaitingPickup;
                    booking.UpdatedAt = DateTime.UtcNow;

                    // Log status history
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
                        if (overlap.Id != booking.Id && overlap.Status == BookingStatus.PendingApproval)
                        {
                            var overlapOldStatus = overlap.Status;
                            overlap.Status = BookingStatus.Rejected;
                            overlap.CancellationReason = "Tự động từ chối do trùng lịch với đơn đặt xe khác đã đặt cọc.";
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
                                if (voucher != null && voucher.UsedCount > 0)
                                {
                                    voucher.UsedCount--;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Failed payment
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
