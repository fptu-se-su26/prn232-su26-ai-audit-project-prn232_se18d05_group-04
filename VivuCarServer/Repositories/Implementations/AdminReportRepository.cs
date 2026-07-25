using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations;

public class AdminReportRepository(VivuCarDbContext dbContext) : IAdminReportRepository
{
    public async Task<IReadOnlyList<DailyRevenueSnapshot>> GetRevenueSnapshotsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyRevenueSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.SnapshotDate >= from && snapshot.SnapshotDate <= to)
            .OrderBy(snapshot => snapshot.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminHourlyRevenueRecord>> GetRevenueByHourAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        return await dbContext.PaymentTransactions
            .AsNoTracking()
            .Where(payment =>
                payment.Status == PaymentStatus.Success
                && payment.PaidAt.HasValue
                && payment.PaidAt.Value >= start
                && payment.PaidAt.Value < endExclusive)
            .GroupBy(payment => payment.PaidAt!.Value.Hour)
            .Select(group => new AdminHourlyRevenueRecord
            {
                Hour = group.Key,
                GrossRevenue = group.Sum(payment => payment.Amount),
                PaidTransactions = group.Count()
            })
            .OrderBy(item => item.Hour)
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<AdminRevenueStatusRecord>> GetRevenueStatusByDateAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var start = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.PaymentTransactions)
            .Where(booking => booking.CreatedAt >= start && booking.CreatedAt < endExclusive)
            .ToListAsync(cancellationToken);

        return bookings
            .GroupBy(booking => DateOnly.FromDateTime(booking.CreatedAt))
            .Select(group => new AdminRevenueStatusRecord
            {
                Date = group.Key,
                PendingPaymentAmount = group
                    .Where(booking => booking.Status is not BookingStatus.Cancelled and not BookingStatus.Expired)
                    .Sum(booking => Math.Max(
                        0,
                        booking.TotalAmount - booking.PaymentTransactions
                            .Where(payment => payment.Status == PaymentStatus.Success)
                            .Sum(payment => payment.Amount))),
                CancelledAmount = group
                    .Where(booking => booking.Status is BookingStatus.Cancelled or BookingStatus.Expired
                    && !booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success))
                    .Sum(booking => Math.Max(
                        0,
                        booking.TotalAmount - booking.PaymentTransactions
                            .Where(payment => payment.Status == PaymentStatus.Success)
                            .Sum(payment => payment.Amount))),
                PaidBookings = group.Count(booking => booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success)),
                PendingPaymentBookings = group.Count(booking =>
                    booking.Status is not BookingStatus.Cancelled and not BookingStatus.Expired
                    && !booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success)),
                CancelledWithoutPaymentBookings = group.Count(booking =>
                    booking.Status is BookingStatus.Cancelled or BookingStatus.Expired
                    && !booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success))
            })
            .OrderBy(item => item.Date)
            .ToList();
    }

    public async Task<int> CountBookingsAsync(
        DateOnly from,
        DateOnly to,
        string? search = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        CancellationToken cancellationToken = default)
    {
        return await BuildBookingQuery(from, to, search, bookingStatus, paymentStatus)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminRecentBookingRecord>> GetBookingsPageAsync(
        DateOnly from,
        DateOnly to,
        int skip,
        int take,
        string? search = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        CancellationToken cancellationToken = default)
    {
        var bookings = await BuildBookingQuery(from, to, search, bookingStatus, paymentStatus)
            .Include(booking => booking.Customer)
            .Include(booking => booking.Car)
            .Include(booking => booking.PaymentTransactions)
            .OrderByDescending(booking => booking.CreatedAt)
            .ThenByDescending(booking => booking.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Clamp(take, 1, 50))
            .ToListAsync(cancellationToken);

        return bookings.Select(booking =>
        {
            var payment = booking.PaymentTransactions
                .OrderByDescending(item => item.PaidAt ?? item.CreatedAt)
                .ThenByDescending(item => item.Id)
                .FirstOrDefault();

            return new AdminRecentBookingRecord
            {
                Id = booking.Id,
                BookingCode = booking.BookingCode,
                CustomerName = booking.Customer.FullName,
                CarName = booking.Car.Name,
                PickupDate = booking.StartDateTime,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.Status.ToString(),
                PaymentStatus = payment?.Status.ToString() ?? "Pending"
            };
        }).ToList();
    }

    private IQueryable<Booking> BuildBookingQuery(
        DateOnly from,
        DateOnly to,
        string? search,
        string? bookingStatus,
        string? paymentStatus)
    {
        var start = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var query = dbContext.Bookings
            .AsNoTracking()
            .Where(booking => booking.CreatedAt >= start && booking.CreatedAt < endExclusive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(booking =>
                booking.BookingCode.Contains(term) || booking.Customer.FullName.Contains(term));
        }

        query = bookingStatus?.Trim().ToLowerInvariant() switch
        {
            "pending" => query.Where(booking =>
                booking.Status == BookingStatus.PendingApproval || booking.Status == BookingStatus.WaitingDeposit),
            "approved" => query.Where(booking =>
                booking.Status == BookingStatus.WaitingPickup
                || booking.Status == BookingStatus.InProgress
                || booking.Status == BookingStatus.ReturnRequested),
            "rejected" => query.Where(booking => booking.Status == BookingStatus.Rejected),
            "completed" => query.Where(booking => booking.Status == BookingStatus.Completed),
            "cancelled" => query.Where(booking =>
                booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Expired),
            _ => query
        };

        if (!string.IsNullOrWhiteSpace(paymentStatus))
        {
            var normalizedPaymentStatus = paymentStatus.Trim().ToLowerInvariant();
            query = normalizedPaymentStatus switch
            {
                "success" => query.Where(booking =>
                    booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success)),
                "pending" => query.Where(booking =>
                    !booking.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success)
                    && (!booking.PaymentTransactions.Any()
                        || booking.PaymentTransactions
                            .OrderByDescending(payment => payment.PaidAt ?? payment.CreatedAt)
                            .ThenByDescending(payment => payment.Id)
                            .Select(payment => payment.Status)
                            .FirstOrDefault() == PaymentStatus.Pending)),
                "failed" => query.Where(booking =>
                    booking.PaymentTransactions.Any()
                    && (booking.PaymentTransactions
                        .OrderByDescending(payment => payment.PaidAt ?? payment.CreatedAt)
                        .ThenByDescending(payment => payment.Id)
                        .Select(payment => payment.Status)
                        .FirstOrDefault() == PaymentStatus.Failed
                        || booking.PaymentTransactions
                            .OrderByDescending(payment => payment.PaidAt ?? payment.CreatedAt)
                            .ThenByDescending(payment => payment.Id)
                            .Select(payment => payment.Status)
                            .FirstOrDefault() == PaymentStatus.Cancelled)),
                // The current backend enum has no Refunded value yet; do not map it to a different persisted state.
                "refunded" => query.Where(_ => false),
                _ => query
            };
        }

        return query;
    }
}