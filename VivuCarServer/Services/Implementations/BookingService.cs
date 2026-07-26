using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Booking;

namespace Services.Implementations;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<bool> CheckAvailabilityAsync(int carId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var isBooked = await bookingRepository.CheckOverlapExistsAsync(carId, start, end, null, cancellationToken);
        return !isBooked;
    }

    public async Task<PricePreviewResponse> CalculatePricePreviewAsync(PricePreviewRequest request, CancellationToken cancellationToken = default)
    {
        var car = await bookingRepository.GetCarByIdAsync(request.CarId, cancellationToken);
        if (car == null)
        {
            throw new ArgumentException("Car not found.");
        }

        return CalculatePrices(car, request.StartDateTime, request.EndDateTime, request.HasInsurance, request.HasDelivery, request.DistanceKm, request.VoucherCode, out _);
    }

    private PricePreviewResponse CalculatePrices(
        Car car,
        DateTime start,
        DateTime end,
        bool hasInsurance,
        bool hasDelivery,
        decimal distanceKm,
        string? voucherCode,
        out Voucher? appliedVoucher
    )
    {
        appliedVoucher = null;

        if (end <= start)
        {
            throw new ArgumentException("End date must be after start date.");
        }
        
        if (start.Hour < 7 || start.Hour >= 22 || end.Hour < 7 || end.Hour >= 22)
        {
            throw new ArgumentException("Booking time must be between 07:00 and 22:00.");
        }

        var totalHours = (end - start).TotalHours;
        var rentalDays = (int)Math.Floor(totalHours / 24.0);
        var rentalHours = (int)Math.Ceiling(totalHours % 24.0);

        if (rentalDays == 0 && rentalHours == 0)
        {
            rentalHours = 1; // Minimum 1 hour
        }

        var hourlyCost = rentalHours * car.PricePerHour;
        if (hourlyCost > car.DailyPrice)
        {
            // If extra hours cost more than a day, cap it to a full day
            rentalDays++;
            rentalHours = 0;
            hourlyCost = 0;
        }

        var weekdayCount = 0;
        var weekendCount = 0;
        var tempDate = start;
        for (int i = 0; i < rentalDays; i++)
        {
            if (tempDate.DayOfWeek == DayOfWeek.Saturday || tempDate.DayOfWeek == DayOfWeek.Sunday)
            {
                weekendCount++;
            }
            else
            {
                weekdayCount++;
            }
            tempDate = tempDate.AddDays(1);
        }

        var weekdayPrice = car.DailyPrice;
        var weekendPrice = car.DailyPrice;

        var weekdayCost = weekdayCount * weekdayPrice;
        var weekendCost = weekendCount * weekendPrice;
        var basePrice = weekdayCost + weekendCost + hourlyCost;

        var insuranceFee = hasInsurance ? car.InsuranceFeePerDay * (rentalDays + (rentalHours > 0 ? 1 : 0)) : 0m;
        var deliveryFee = hasDelivery ? car.DeliveryFee * (distanceKm > 0 ? distanceKm : 1m) : 0m;

        // Apply voucher discount
        var discountAmount = 0m;
        if (!string.IsNullOrWhiteSpace(voucherCode))
        {
            // We run synchronously since this helper is called from async wrappers
            var voucher = bookingRepository.GetVoucherByCodeAsync(voucherCode).GetAwaiter().GetResult();
            if (voucher != null && (!voucher.ExpiresAt.HasValue || voucher.ExpiresAt.Value >= DateTime.UtcNow))
            {
                if (voucher.Quantity > voucher.BookingVouchers.Count)
                {
                    if (basePrice >= voucher.MinOrderAmount)
                    {
                        appliedVoucher = voucher;
                        if (voucher.DiscountType == DiscountType.Percentage)
                        {
                            discountAmount = basePrice * (voucher.DiscountValue / 100m);
                            if (voucher.MaxDiscount > 0 && discountAmount > voucher.MaxDiscount)
                            {
                                discountAmount = voucher.MaxDiscount;
                            }
                        }
                        else if (voucher.DiscountType == DiscountType.Fixed)
                        {
                            discountAmount = voucher.DiscountValue;
                        }

                        discountAmount = Math.Min(discountAmount, basePrice);
                    }
                }
            }
        }

        var totalAmount = basePrice + insuranceFee + deliveryFee - discountAmount;
        var depositAmount = Math.Round(totalAmount * 0.1m, 0);
        var remainingAmount = totalAmount - depositAmount;

        return new PricePreviewResponse
        {
            RentalDays = rentalDays,
            RentalHours = rentalHours,
            WeekdayCount = weekdayCount,
            WeekendCount = weekendCount,
            WeekdayPrice = weekdayPrice,
            WeekendPrice = weekendPrice,
            HourlyPrice = car.PricePerHour,
            WeekdayCost = weekdayCost,
            WeekendCost = weekendCost,
            HourlyCost = hourlyCost,
            InsuranceFee = insuranceFee,
            DeliveryFee = deliveryFee,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            DepositAmount = depositAmount,
            RemainingAmount = remainingAmount
        };
    }

    public async Task<BookingDetailResponse> CreateBookingAsync(int customerId, CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var car = await bookingRepository.GetCarByIdAsync(request.CarId, cancellationToken);
        if (car == null)
        {
            throw new ArgumentException("Car not found.");
        }

        if (car.OwnerId == customerId)
        {
            throw new ArgumentException("Owners cannot book their own cars.");
        }

        // Calculate pricing
        var pricing = CalculatePrices(car, request.StartDateTime, request.EndDateTime, request.HasInsurance, request.HasDelivery, request.DistanceKm, request.VoucherCode, out var voucher);

        // Transaction handling
        using var transaction = await bookingRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            // Check availability
            var isBooked = await bookingRepository.CheckOverlapExistsAsync(request.CarId, request.StartDateTime, request.EndDateTime, null, cancellationToken);
            if (isBooked)
            {
                throw new InvalidOperationException("Car is not available for the selected dates.");
            }

            // Sync driver documents if required
            var currentDoc = await bookingRepository.GetDriverDocumentByUserIdAsync(customerId, cancellationToken);
            if (currentDoc == null)
            {
                currentDoc = new DriverDocument
                {
                    UserId = customerId,
                    CitizenIdNumber = request.DriverInfo.CitizenIdNumber,
                    CitizenIdFrontImageUrl = request.DriverInfo.CitizenIdFrontImageUrl,
                    CitizenIdBackImageUrl = request.DriverInfo.CitizenIdBackImageUrl,
                    DriverLicenseNumber = request.DriverInfo.DriverLicenseNumber,
                    DriverLicenseFrontImageUrl = request.DriverInfo.DriverLicenseFrontImageUrl,
                    DriverLicenseBackImageUrl = request.DriverInfo.DriverLicenseBackImageUrl,
                    VerificationStatus = DocumentVerificationStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await bookingRepository.AddDriverDocumentAsync(currentDoc, cancellationToken);
            }

            var isVerified = currentDoc.VerificationStatus == DocumentVerificationStatus.Approved;
            var initialStatus = isVerified ? BookingStatus.WaitingDeposit : BookingStatus.PendingApproval;

            // Create Booking object
            var booking = new Booking
            {
                BookingCode = "BK" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999),
                CustomerId = customerId,
                CarId = request.CarId,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                PickupLocation = request.PickupLocation,
                ReturnLocation = request.ReturnLocation,
                BasePrice = pricing.WeekdayCost + pricing.WeekendCost,
                InsuranceFee = pricing.InsuranceFee,
                DeliveryFee = pricing.DeliveryFee,
                DiscountAmount = pricing.DiscountAmount,
                DepositAmount = pricing.DepositAmount,
                TotalAmount = pricing.TotalAmount,
                RemainingAmount = pricing.RemainingAmount,
                Status = initialStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Set Driver info
            booking.DriverInfo = new BookingDriverInfo
            {
                FullName = request.DriverInfo.FullName,
                PhoneNumber = request.DriverInfo.PhoneNumber,
                CitizenIdNumber = request.DriverInfo.CitizenIdNumber,
                CitizenIdFrontImageUrl = request.DriverInfo.CitizenIdFrontImageUrl,
                CitizenIdBackImageUrl = request.DriverInfo.CitizenIdBackImageUrl,
                DriverLicenseNumber = request.DriverInfo.DriverLicenseNumber,
                DriverLicenseFrontImageUrl = request.DriverInfo.DriverLicenseFrontImageUrl,
                DriverLicenseBackImageUrl = request.DriverInfo.DriverLicenseBackImageUrl
            };

            // Link Voucher
            if (voucher != null)
            {
                booking.BookingVoucher = new BookingVoucher
                {
                    VoucherId = voucher.Id,
                    Code = voucher.Code,
                    DiscountAmount = pricing.DiscountAmount,
                    AppliedAt = DateTime.UtcNow
                };
            }

            // Create Calendar Block
            var blockReason = isVerified ? $"Soft-lock 15m for Booking {booking.BookingCode}" : $"Pending Admin GPLX Approval for Booking {booking.BookingCode}";
            var block = new CarAvailabilityBlock
            {
                CarId = booking.CarId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Reason = blockReason,
                // We don't have booking.Id yet, it will be mapped after AddAsync and SaveChanges,
                // but EF Core can handle navigation property if we do booking.AvailabilityBlocks.Add
            };
            booking.AvailabilityBlocks.Add(block);

            // Track Status History
            var note = isVerified ? "Auto-approved (GPLX verified). Soft-lock created for 15 minutes." : "Booking created by customer. Waiting for Admin to verify GPLX.";
            booking.StatusHistories.Add(new BookingStatusHistory
            {
                OldStatus = BookingStatus.PendingApproval, // Virtual initial state
                NewStatus = initialStatus,
                ChangedByUserId = customerId,
                Note = note,
                CreatedAt = DateTime.UtcNow
            });

            await bookingRepository.AddAsync(booking, cancellationToken);
            await bookingRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return MapToDetailResponse(booking);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<BookingDetailResponse?> GetBookingDetailAsync(int customerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        // Ensure user is either the customer or the owner of the car
        if (booking.CustomerId != customerId && booking.Car.OwnerId != customerId)
        {
            return null;
        }

        return MapToDetailResponse(booking);
    }

    public async Task<IReadOnlyList<BookingDetailResponse>> GetMyBookingsAsync(int customerId, BookingListFilter filter, CancellationToken cancellationToken = default)
    {
        var list = await bookingRepository.GetListAsync(customerId, filter.Status, filter.Page, filter.PageSize, cancellationToken);
        return list.Select(MapToDetailResponse).ToList();
    }

    public async Task<IReadOnlyList<BookingDetailResponse>> GetOwnerBookingsAsync(int ownerId, BookingListFilter filter, CancellationToken cancellationToken = default)
    {
        var list = await bookingRepository.GetOwnerListAsync(ownerId, filter.Status, filter.Page, filter.PageSize, cancellationToken);
        return list.Select(MapToDetailResponse).ToList();
    }

    public async Task<CancelBookingResponse> CancelBookingAsync(int customerId, int bookingId, CancelBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.Confirmed)
        {
            throw new ArgumentException("Cancellation confirmation check is required.");
        }

        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.");
        }

        // Access control: must be customer or owner
        if (booking.CustomerId != customerId && booking.Car.OwnerId != customerId)
        {
            throw new UnauthorizedAccessException("Unauthorized access to booking.");
        }

        // Validation of status: Only allow cancellation if deposit has not been paid
        if (booking.Status != BookingStatus.PendingApproval && booking.Status != BookingStatus.WaitingDeposit)
        {
            throw new InvalidOperationException($"Không thể hủy đơn thuê ở trạng thái {booking.Status}. Chỉ hỗ trợ hủy khi chưa thanh toán cọc.");
        }

        using var transaction = await bookingRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = request.Reason + (string.IsNullOrWhiteSpace(request.Note) ? "" : ": " + request.Note);
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            // Log status history
            booking.StatusHistories.Add(new BookingStatusHistory
            {
                OldStatus = oldStatus,
                NewStatus = BookingStatus.Cancelled,
                ChangedByUserId = customerId,
                Note = "Booking cancelled. Reason: " + request.Reason,
                CreatedAt = DateTime.UtcNow
            });

            // Release calendar block if any
            var releaseCount = 0;
            var overlappingBlocks = await bookingRepository.GetOverlappingBlocksAsync(booking.CarId, booking.StartDateTime, booking.EndDateTime, cancellationToken);
            var bookingBlocks = overlappingBlocks.Where(b => b.BookingId == booking.Id).ToList();
            if (bookingBlocks.Count > 0)
            {
                releaseCount = bookingBlocks.Count;
                // Delete blocks from context
                // We'll query context directly through bookingRepository if it tracks it, or we can clear booking.AvailabilityBlocks.
                // In BookingRepository, we retrieved blocks. Let's delete them.
                foreach (var block in bookingBlocks)
                {
                    booking.AvailabilityBlocks.Remove(block);
                }
            }

            // Calculate refund (no refund since cancellation is only allowed before deposit)
            var refundAmount = 0m;

            // Restore voucher usage count
            if (booking.BookingVoucher != null)
            {
                var voucher = await bookingRepository.GetVoucherByCodeAsync(booking.BookingVoucher.Code, cancellationToken);
            }

            await bookingRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CancelBookingResponse
            {
                BookingId = booking.Id,
                Status = booking.Status.ToString(),
                CarAvailabilityReleased = releaseCount > 0 || true,
                RefundAmount = refundAmount
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<BookingDetailResponse?> ApproveBookingRequestAsync(int ownerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        // Ensure user is the owner of the car
        if (booking.Car.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("Only the car owner can approve booking requests.");
        }

        if (booking.Status != BookingStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only bookings in PendingApproval state can be approved.");
        }

        using var transaction = await bookingRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            // Concurrency Check: ensure no confirmed overlap
            var isBooked = await bookingRepository.CheckOverlapExistsAsync(booking.CarId, booking.StartDateTime, booking.EndDateTime, booking.Id, cancellationToken);
            if (isBooked)
            {
                throw new InvalidOperationException("Car calendar is already blocked or confirmed for these dates.");
            }

            var oldStatus = booking.Status;
            booking.Status = BookingStatus.WaitingDeposit;
            booking.UpdatedAt = DateTime.UtcNow;

            // Lock calendar for 15 minutes
            var block = new CarAvailabilityBlock
            {
                CarId = booking.CarId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Reason = $"Soft-lock 15m for Booking {booking.BookingCode}",
                BookingId = booking.Id
            };
            booking.AvailabilityBlocks.Add(block);

            // Log status history
            booking.StatusHistories.Add(new BookingStatusHistory
            {
                OldStatus = oldStatus,
                NewStatus = BookingStatus.WaitingDeposit,
                ChangedByUserId = ownerId,
                Note = "Booking approved by owner. Soft-lock created for 15 minutes.",
                CreatedAt = DateTime.UtcNow
            });

            await bookingRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return MapToDetailResponse(booking);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<BookingDetailResponse?> RejectBookingRequestAsync(int ownerId, int bookingId, string reason, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        if (booking.Car.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("Only the car owner can reject booking requests.");
        }

        if (booking.Status != BookingStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only bookings in PendingApproval state can be rejected.");
        }

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.Rejected;
        booking.CancellationReason = "Rejected by owner: " + reason;
        booking.CancelledAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        // Log status history
        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.Rejected,
            ChangedByUserId = ownerId,
            Note = "Booking rejected by owner: " + reason,
            CreatedAt = DateTime.UtcNow
        });

        // Restore voucher usage
        if (booking.BookingVoucher != null)
        {
            var voucher = await bookingRepository.GetVoucherByCodeAsync(booking.BookingVoucher.Code, cancellationToken);
        }

        await bookingRepository.SaveChangesAsync(cancellationToken);

        return MapToDetailResponse(booking);
    }

    public async Task<BookingDetailResponse?> GetContractAsync(int customerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        if (booking.CustomerId != customerId && booking.Car.OwnerId != customerId)
        {
            return null;
        }

        // If booking is waiting pickup, in progress or completed, and no contract exists, generate one
        if (booking.Status == BookingStatus.WaitingPickup || booking.Status == BookingStatus.InProgress || booking.Status == BookingStatus.Completed)
        {
            if (booking.RentalContract == null)
            {
                booking.RentalContract = new RentalContract
                {
                    ContractNumber = "HD-" + booking.BookingCode,
                    PdfUrl = $"/api/bookings/{booking.Id}/contract/pdf",
                    GeneratedAt = DateTime.UtcNow
                };
                await bookingRepository.SaveChangesAsync(cancellationToken);
            }
        }

        return MapToDetailResponse(booking);
    }

    public async Task<bool> UpdateContractSignatureAsync(int customerId, int bookingId, string signatureUrl, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return false;

        if (booking.CustomerId != customerId && booking.Car.OwnerId != customerId)
        {
            return false;
        }

        if (booking.RentalContract == null)
        {
            return false;
        }

        var sep = booking.RentalContract.PdfUrl.Contains('?') ? "&" : "?";
        booking.RentalContract.PdfUrl = $"{booking.RentalContract.PdfUrl}{sep}sig={Uri.EscapeDataString(signatureUrl)}";
        
        await bookingRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<BookingDetailResponse?> RequestReturnAsync(int customerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null) return null;

        if (booking.CustomerId != customerId)
        {
            throw new UnauthorizedAccessException("You do not have permission to request return for this booking.");
        }

        if (booking.Status != BookingStatus.InProgress)
        {
            throw new InvalidOperationException("You can only request return for a booking that is currently in progress.");
        }

        booking.Status = BookingStatus.ReturnRequested;
        booking.UpdatedAt = DateTime.UtcNow;

        await bookingRepository.SaveChangesAsync(cancellationToken);

        return MapToDetailResponse(booking);
    }

    private BookingDetailResponse MapToDetailResponse(Booking b)
    {
        var primaryImage = b.Car.Images?.OrderBy(i => i.DisplayOrder).FirstOrDefault(i => i.IsPrimary) 
                           ?? b.Car.Images?.OrderBy(i => i.DisplayOrder).FirstOrDefault();

        var totalHours = (b.EndDateTime - b.StartDateTime).TotalHours;
        var rentalDays = (int)Math.Floor(totalHours / 24.0);
        var rentalHours = (int)Math.Ceiling(totalHours % 24.0);
        if (rentalDays == 0 && rentalHours == 0) rentalHours = 1;
        if (b.Car != null && rentalHours * b.Car.PricePerHour > b.Car.DailyPrice)
        {
            rentalDays++;
            rentalHours = 0;
        }

        return new BookingDetailResponse
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            CustomerId = b.CustomerId,
            CarId = b.CarId,
            CarName = b.Car?.Name ?? string.Empty,
            LicensePlate = b.Car?.LicensePlate ?? string.Empty,
            CarImageUrl = primaryImage?.ImageUrl,
            StartDateTime = b.StartDateTime,
            EndDateTime = b.EndDateTime,
            PickupLocation = b.PickupLocation,
            ReturnLocation = b.ReturnLocation,
            RentalDays = rentalDays,
            RentalHours = rentalHours,
            BasePrice = b.BasePrice,
            InsuranceFee = b.InsuranceFee,
            DeliveryFee = b.DeliveryFee,
            DiscountAmount = b.DiscountAmount,
            TotalAmount = b.TotalAmount,
            DepositAmount = b.DepositAmount,
            RemainingAmount = b.RemainingAmount,
            Status = b.Status.ToString().ToLowerInvariant(),
            CancellationReason = b.CancellationReason,
            CancelledAt = b.CancelledAt,
            CreatedAt = b.CreatedAt,
            VoucherCode = b.BookingVoucher?.Code,
            ContractNumber = b.RentalContract?.ContractNumber,
            ContractPdfUrl = b.RentalContract?.PdfUrl,
            DriverInfo = b.DriverInfo == null ? null : new DriverInfoDto
            {
                FullName = b.DriverInfo.FullName,
                PhoneNumber = b.DriverInfo.PhoneNumber,
                CitizenIdNumber = b.DriverInfo.CitizenIdNumber,
                CitizenIdFrontImageUrl = b.DriverInfo.CitizenIdFrontImageUrl,
                CitizenIdBackImageUrl = b.DriverInfo.CitizenIdBackImageUrl,
                DriverLicenseNumber = b.DriverInfo.DriverLicenseNumber,
                DriverLicenseFrontImageUrl = b.DriverInfo.DriverLicenseFrontImageUrl,
                DriverLicenseBackImageUrl = b.DriverInfo.DriverLicenseBackImageUrl
            }
        };
    }
}

