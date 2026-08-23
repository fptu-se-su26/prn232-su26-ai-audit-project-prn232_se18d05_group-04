using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Booking?> GetByTransactionCodeAsync(string transactionCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetListAsync(
        int customerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<Booking>> GetOwnerListAsync(
        int ownerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<bool> CheckOverlapExistsAsync(
        int carId,
        DateTime start,
        DateTime end,
        int? excludeBookingId = null,
        CancellationToken cancellationToken = default
    );

    Task<List<CarAvailabilityBlock>> GetOverlappingBlocksAsync(
        int carId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    );

    Task<List<Booking>> GetOverlappingBookingsAsync(
        int carId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    );

    Task<Voucher?> GetVoucherByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<Car?> GetCarByIdAsync(int carId, CancellationToken cancellationToken = default);

    Task<DriverDocument?> GetDriverDocumentByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task AddDriverDocumentAsync(DriverDocument doc, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    // Context access for transactions
    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? BeginTransaction();
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
