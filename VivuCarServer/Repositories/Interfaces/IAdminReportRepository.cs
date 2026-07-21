using BusinessObjects.Models;
using Repositories.Models;

namespace Repositories.Interfaces;

public interface IAdminReportRepository
{
    Task<IReadOnlyList<DailyRevenueSnapshot>> GetRevenueSnapshotsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<int> CountBookingsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminRecentBookingRecord>> GetBookingsPageAsync(DateOnly from, DateOnly to, int skip, int take, CancellationToken cancellationToken = default);
}
