using BusinessObjects.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations;

public class AdminReportRepository(VivuCarDbContext dbContext) : IAdminReportRepository
{
    public async Task<IReadOnlyList<BusinessObjects.Models.DailyRevenueSnapshot>> GetRevenueSnapshotsAsync(
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

    public async Task<int> CountBookingsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var start = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        return await dbContext.Bookings
            .AsNoTracking()
            .CountAsync(
                booking => booking.CreatedAt >= start && booking.CreatedAt < endExclusive,
                cancellationToken);
    }

    public async Task<IReadOnlyList<AdminRecentBookingRecord>> GetBookingsPageAsync(
        DateOnly from,
        DateOnly to,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var start = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.Customer)
            .Include(booking => booking.Car)
            .Include(booking => booking.PaymentTransactions)
            .Where(booking => booking.CreatedAt >= start && booking.CreatedAt < endExclusive)
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
}
