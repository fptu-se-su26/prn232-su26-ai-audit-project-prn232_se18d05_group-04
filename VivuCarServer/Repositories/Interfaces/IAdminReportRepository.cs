using BusinessObjects.Models;
using Repositories.Models;

namespace Repositories.Interfaces;

public interface IAdminReportRepository
{
    Task<IReadOnlyList<DailyRevenueSnapshot>> GetRevenueSnapshotsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminHourlyRevenueRecord>> GetRevenueByHourAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminRevenueStatusRecord>> GetRevenueStatusByDateAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<int> CountBookingsAsync(
        DateOnly from,
        DateOnly to,
        string? search = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminRecentBookingRecord>> GetBookingsPageAsync(
        DateOnly from,
        DateOnly to,
        int skip,
        int take,
        string? search = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        CancellationToken cancellationToken = default);
}