using Repositories.Models;
using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminExportService
{
    Task<AdminReportPreviewResponse> PreviewAsync(AdminReportFilter filter, CancellationToken cancellationToken = default);
    Task<AdminExportJobResponse> CreateJobAsync(int adminId, AdminExportJobCreateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminExportJobResponse>> GetJobsAsync(int adminId, CancellationToken cancellationToken = default);
    Task<AdminExportJobResponse?> GetJobAsync(int adminId, int id, CancellationToken cancellationToken = default);
    Task<ExportFileContent?> DownloadAsync(int adminId, int id, CancellationToken cancellationToken = default);
}

public interface IAdminExportFileStore
{
    Task<string> SaveAsync(string fileName, byte[] content, CancellationToken cancellationToken = default);
    Task<byte[]?> ReadAsync(string relativePath, CancellationToken cancellationToken = default);
}

