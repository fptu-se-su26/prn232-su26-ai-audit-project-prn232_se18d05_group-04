using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminReportService
{
    Task<AdminRevenueReportResponse> GetRevenueAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}
