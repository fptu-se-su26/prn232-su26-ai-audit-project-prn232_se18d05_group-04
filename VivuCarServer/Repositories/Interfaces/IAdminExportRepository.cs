using BusinessObjects.Models;
using Repositories.Models;

namespace Repositories.Interfaces;

public interface IAdminExportRepository
{
    Task<AdminReportPreviewResponse> GetPreviewAsync(AdminReportFilter filter, int limit, CancellationToken cancellationToken = default);
    Task AddJobAsync(ExportJob job, CancellationToken cancellationToken = default);
    Task<ExportJob?> GetJobAsync(int id, int adminId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExportJob>> GetJobsAsync(int adminId, int limit, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

