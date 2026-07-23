using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminReportService
{
    Task<AdminRevenueReportResponse> GetRevenueAsync(
        DateOnly from,
        DateOnly to,
        int page = 1,
        int pageSize = 5,
        CancellationToken cancellationToken = default);
}
