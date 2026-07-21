using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IAdminVoucherRepository
{
    IQueryable<Voucher> Query();
    Task<Voucher?> GetByIdAsync(int id, bool includeUsage = false, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default);
    void Remove(Voucher voucher);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
