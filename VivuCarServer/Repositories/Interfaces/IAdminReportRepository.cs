using BusinessObjects.Models;
using Repositories.Models;

namespace Repositories.Interfaces;

public interface IAdminReportRepository
{
    Task<IReadOnlyList<DailyRevenueSnapshot>> GetRevenueSnapshotsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminRecentBookingRecord>> GetRecentBookingsAsync(DateOnly from, DateOnly to, int limit, CancellationToken cancellationToken = default);
}
