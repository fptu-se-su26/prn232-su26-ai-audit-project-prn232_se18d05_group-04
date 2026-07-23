using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminVoucherService
{
    Task<PagedResult<AdminVoucherResponse>> GetListAsync(AdminVoucherListQuery query, CancellationToken cancellationToken = default);
    Task<AdminVoucherResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminVoucherResponse> CreateAsync(AdminVoucherUpsertRequest request, CancellationToken cancellationToken = default);
    Task<AdminVoucherResponse?> UpdateAsync(int id, AdminVoucherUpsertRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminVoucherPerformanceResponse?> GetPerformanceAsync(int id, CancellationToken cancellationToken = default);
}

public class AdminVoucherServiceException(int statusCode, string message, IReadOnlyDictionary<string, string[]>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;
}
