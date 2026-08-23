using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class AdminVoucherRepository(VivuCarDbContext dbContext) : IAdminVoucherRepository
{
    public IQueryable<Voucher> Query() => dbContext.Vouchers
        .AsNoTracking()
        .Include(voucher => voucher.BookingVouchers);

    public Task<Voucher?> GetByIdAsync(int id, bool includeUsage = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Voucher> query = dbContext.Vouchers;
        if (includeUsage)
        {
            query = query.Include(voucher => voucher.BookingVouchers)
                .ThenInclude(usage => usage.Booking)
                .ThenInclude(booking => booking.Customer);
        }
        return query.SingleOrDefaultAsync(voucher => voucher.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
        => dbContext.Vouchers.AnyAsync(voucher => voucher.Code == code && (!excludeId.HasValue || voucher.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default)
        => dbContext.Vouchers.AddAsync(voucher, cancellationToken).AsTask();

    public void Remove(Voucher voucher) => dbContext.Vouchers.Remove(voucher);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
