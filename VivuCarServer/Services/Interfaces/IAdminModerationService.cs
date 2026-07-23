using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminModerationService
{
    Task<IReadOnlyList<AdminModerationContentResponse>> GetContentAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminLicenseModerationResponse>> GetLicensesAsync(CancellationToken cancellationToken = default);
    Task<AdminLicenseOcrResponse?> ScanLicenseAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SetLicenseStatusAsync(int id, string status, CancellationToken cancellationToken = default);
}
