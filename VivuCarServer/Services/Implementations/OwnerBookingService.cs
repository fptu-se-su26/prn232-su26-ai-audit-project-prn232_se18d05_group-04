using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Owner;
using Services.Models;

namespace Services.Implementations;

public class OwnerBookingService(VivuCarDbContext dbContext) : IOwnerBookingService
{
    public async Task<PagedResult<OwnerBookingResponse>> GetOwnerBookingsAsync(
        int ownerId,
        OwnerBookingListFilter filter,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Where(b => b.Car.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(filter.Status) && filter.Status.ToUpper() != "ALL")
        {
            var mappedStatuses = filter.Status.ToLowerInvariant() switch
            {
                "pending" => new[] { BookingStatus.PendingApproval, BookingStatus.WaitingDeposit },
                "approved" => new[] { BookingStatus.WaitingPickup, BookingStatus.InProgress, BookingStatus.ReturnRequested },
                "completed" => new[] { BookingStatus.Completed },
                "rejected" => new[] { BookingStatus.Rejected },
                "cancelled" => new[] { BookingStatus.Cancelled, BookingStatus.Expired },
                _ => Array.Empty<BookingStatus>()
            };

            if (mappedStatuses.Length > 0)
            {
                query = query.Where(b => mappedStatuses.Contains(b.Status));
            }
            else if (Enum.TryParse<BookingStatus>(filter.Status, true, out var statusEnum))
            {
                query = query.Where(b => b.Status == statusEnum);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var page = Math.Max(filter.Page, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var list = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OwnerBookingResponse>
        {
            Items = list.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OwnerBookingDetailResponse?> GetOwnerBookingDetailAsync(
        int ownerId,
        int bookingId,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.DriverInfo)
            .Include(b => b.RentalContract)
            .Include(b => b.BookingVoucher)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerBookingDetailResponse?> ConfirmHandoverAsync(
        int ownerId,
        int bookingId,
        HandoverRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.DriverInfo)
            .Include(b => b.StatusHistories)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;

        if (booking.Status != BookingStatus.WaitingPickup)
            throw new InvalidOperationException("Chỉ có thể bàn giao xe ở trạng thái Chờ nhận xe.");

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.InProgress;
        booking.UpdatedAt = DateTime.UtcNow;

        // Update car status to Rented
        booking.Car.Status = CarStatus.Rented;
        booking.Car.UpdatedAt = DateTime.UtcNow;

        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.InProgress,
            ChangedByUserId = ownerId,
            Note = $"Đã bàn giao xe. Km: {request.OdometerKm}. Nhiên liệu: {request.FuelLevel}. Ngoại thất: {request.ExteriorStatus}. Nội thất: {request.InteriorStatus}. Ghi chú: {request.Note}",
            // UI-only field. Not present in current DB schema. Requires migration before backend integration.
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerBookingDetailResponse?> ConfirmReturnAsync(
        int ownerId,
        int bookingId,
        ReturnInspectionRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.DriverInfo)
            .Include(b => b.StatusHistories)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;

        if (booking.Status != BookingStatus.ReturnRequested && booking.Status != BookingStatus.InProgress)
            throw new InvalidOperationException("Chỉ có thể xác nhận trả xe khi xe đang trong chuyến hoặc đã có yêu cầu trả.");

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        // Update car status based on inspection result
        var nextStatus = ParseOwnerCarStatus(request.NextCarStatus);
        booking.Car.Status = nextStatus;
        booking.Car.UpdatedAt = DateTime.UtcNow;

        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.Completed,
            ChangedByUserId = ownerId,
            Note = $"Xác nhận trả xe. Km: {request.OdometerKm}. Phụ phí: {request.ExtraFee:N0}đ. Hư hỏng: {request.DamageNotes ?? "Không"}",
            CreatedAt = DateTime.UtcNow
        });

        if (!string.IsNullOrWhiteSpace(request.DamageNotes))
        {
            var imagesPart = request.ImageUrls != null && request.ImageUrls.Count > 0
                ? " ||IMAGES|| " + string.Join(",", request.ImageUrls)
                : "";

            dbContext.IncidentReports.Add(new IncidentReport
            {
                BookingId = booking.Id,
                ReporterId = ownerId,
                Title = "Phát sinh hư hỏng lúc nhận xe",
                Description = request.DamageNotes + imagesPart,
                Status = IncidentStatus.Open,
                CreatedAt = DateTime.UtcNow
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerBookingDetailResponse?> ApproveBookingAsync(
        int ownerId,
        int bookingId,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.StatusHistories)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;

        if (booking.Status != BookingStatus.PendingApproval)
            throw new InvalidOperationException("Chỉ có thể duyệt đơn ở trạng thái Chờ duyệt.");

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.WaitingDeposit;
        booking.UpdatedAt = DateTime.UtcNow;

        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.WaitingDeposit,
            ChangedByUserId = ownerId,
            Note = "Chủ xe đã duyệt đơn thuê xe.",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerBookingDetailResponse?> RejectBookingAsync(
        int ownerId,
        int bookingId,
        string? reason,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.StatusHistories)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;

        if (booking.Status != BookingStatus.PendingApproval)
            throw new InvalidOperationException("Chỉ có thể từ chối đơn ở trạng thái Chờ duyệt.");

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.Rejected;
        booking.CancellationReason = reason;
        booking.UpdatedAt = DateTime.UtcNow;

        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.Rejected,
            ChangedByUserId = ownerId,
            Note = $"Chủ xe từ chối: {reason ?? "Không có lý do cụ thể."}",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerBookingDetailResponse?> CompleteBookingAsync(
        int ownerId,
        int bookingId,
        string nextCarStatus,
        CancellationToken cancellationToken = default
    )
    {
        var booking = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.StatusHistories)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null) return null;
        if (booking.Status == BookingStatus.Completed) return MapToDetailResponse(booking);

        var oldStatus = booking.Status;
        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        booking.Car.Status = ParseOwnerCarStatus(nextCarStatus);
        booking.Car.UpdatedAt = DateTime.UtcNow;

        booking.StatusHistories.Add(new BookingStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = BookingStatus.Completed,
            ChangedByUserId = ownerId,
            Note = "Chủ xe xác nhận hoàn tất chuyến.",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailResponse(booking);
    }

    public async Task<OwnerDashboardStats> GetOwnerDashboardStatsAsync(
        int ownerId,
        CancellationToken cancellationToken = default
    )
    {
        var cars = await dbContext.Cars
            .Where(c => c.OwnerId == ownerId)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var bookings = await dbContext.Bookings
            .Include(b => b.Car).ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Where(b => b.Car.OwnerId == ownerId)
            .ToListAsync(cancellationToken);

        var monthlyRevenue = bookings
            .Where(b => b.Status == BookingStatus.Completed && b.UpdatedAt >= monthStart)
            .Sum(b => b.TotalAmount);

        var recentBookings = bookings
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(MapToResponse)
            .ToList();

        return new OwnerDashboardStats
        {
            TotalCars = cars.Count,
            AvailableCars = cars.Count(c => c.Status == CarStatus.Available),
            RentedCars = cars.Count(c => c.Status == CarStatus.Rented),
            PendingBookings = bookings.Count(b => b.Status == BookingStatus.PendingApproval),
            ActiveBookings = bookings.Count(b => b.Status == BookingStatus.InProgress || b.Status == BookingStatus.ReturnRequested),
            MonthlyRevenue = monthlyRevenue,
            RecentBookings = recentBookings
        };
    }

    // ─── Mappers ──────────────────────────────────────────────────────────────

    private static OwnerBookingResponse MapToResponse(Booking b)
    {
        var primaryImage = b.Car?.Images?.FirstOrDefault(i => i.IsPrimary)
            ?? b.Car?.Images?.OrderBy(i => i.DisplayOrder).FirstOrDefault();

        return new OwnerBookingResponse
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            CustomerName = b.Customer?.FullName ?? "Khách",
            CustomerPhone = b.Customer?.PhoneNumber ?? "",
            CarName = b.Car?.Name ?? "",
            LicensePlate = b.Car?.LicensePlate ?? "",
            CarImageUrl = primaryImage?.ImageUrl,
            StartDateTime = b.StartDateTime,
            EndDateTime = b.EndDateTime,
            TotalAmount = b.TotalAmount,
            DepositAmount = b.DepositAmount,
            Status = b.Status.ToString().ToLowerInvariant(),
            CreatedAt = b.CreatedAt
        };
    }

    private static OwnerBookingDetailResponse MapToDetailResponse(Booking b)
    {
        var response = MapToResponse(b);
        return new OwnerBookingDetailResponse
        {
            Id = response.Id,
            BookingCode = response.BookingCode,
            CustomerName = response.CustomerName,
            CustomerPhone = response.CustomerPhone,
            CarName = response.CarName,
            LicensePlate = response.LicensePlate,
            CarImageUrl = response.CarImageUrl,
            StartDateTime = response.StartDateTime,
            EndDateTime = response.EndDateTime,
            TotalAmount = response.TotalAmount,
            DepositAmount = response.DepositAmount,
            Status = response.Status,
            CreatedAt = response.CreatedAt,
            PickupLocation = b.PickupLocation,
            ReturnLocation = b.ReturnLocation,
            BasePrice = b.BasePrice,
            InsuranceFee = b.InsuranceFee,
            DeliveryFee = b.DeliveryFee,
            DiscountAmount = b.DiscountAmount,
            RemainingAmount = b.RemainingAmount,
            CancellationReason = b.CancellationReason,
            DriverLicenseNumber = b.DriverInfo?.DriverLicenseNumber,
            CitizenIdNumber = b.DriverInfo?.CitizenIdNumber,
            ContractNumber = b.RentalContract?.ContractNumber
        };
    }

    private static CarStatus ParseOwnerCarStatus(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "available" => CarStatus.Available,
        "maintenance" => CarStatus.Maintenance,
        "unavailable" => CarStatus.Unavailable,
        _ => CarStatus.Available
    };
}
