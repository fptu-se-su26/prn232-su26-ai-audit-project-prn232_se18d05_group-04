using BusinessObjects.Data;
using BusinessObjects.Models;
using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class BookingRepository(VivuCarDbContext dbContext) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookings
            .Include(b => b.Car)
                .ThenInclude(c => c.CarBrand)
            .Include(b => b.Car)
                .ThenInclude(c => c.CarModel)
            .Include(b => b.Car)
                .ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Include(b => b.DriverInfo)
            .Include(b => b.BookingVoucher)
                .ThenInclude(bv => bv.Voucher)
            .Include(b => b.PaymentTransactions)
            .Include(b => b.RentalContract)
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Booking?> GetByTransactionCodeAsync(string transactionCode, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookings
            .Include(b => b.Car)
            .Include(b => b.Customer)
            .Include(b => b.DriverInfo)
            .Include(b => b.BookingVoucher)
                .ThenInclude(bv => bv.Voucher)
            .Include(b => b.PaymentTransactions)
            .Include(b => b.RentalContract)
            .Include(b => b.AvailabilityBlocks)
            .SingleOrDefaultAsync(b => b.PaymentTransactions.Any(pt => pt.TransactionCode == transactionCode), cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetListAsync(
        int customerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Bookings
            .Include(b => b.Car)
                .ThenInclude(c => c.Images)
            .Where(b => b.CustomerId == customerId);

        if (!string.IsNullOrWhiteSpace(status) && status.ToUpper() != "ALL")
        {
            if (Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
            {
                query = query.Where(b => b.Status == bookingStatus);
            }
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetOwnerListAsync(
        int ownerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Bookings
            .Include(b => b.Car)
                .ThenInclude(c => c.Images)
            .Include(b => b.Customer)
            .Where(b => b.Car.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(status) && status.ToUpper() != "ALL")
        {
            if (Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
            {
                query = query.Where(b => b.Status == bookingStatus);
            }
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public async Task<bool> CheckOverlapExistsAsync(
        int carId,
        DateTime start,
        DateTime end,
        int? excludeBookingId = null,
        CancellationToken cancellationToken = default
    )
    {
        var hasBlock = await dbContext.CarAvailabilityBlocks
            .AnyAsync(b => b.CarId == carId
                           && b.StartDateTime < end
                           && b.EndDateTime > start
                           && (excludeBookingId == null || b.BookingId != excludeBookingId),
                      cancellationToken);

        if (hasBlock) return true;

        var hasBooking = await dbContext.Bookings
            .AnyAsync(b => b.CarId == carId
                           && b.Status != BookingStatus.Cancelled
                           && b.Status != BookingStatus.Rejected
                           && b.Status != BookingStatus.Expired
                           && b.StartDateTime < end
                           && b.EndDateTime > start
                           && (excludeBookingId == null || b.Id != excludeBookingId),
                      cancellationToken);

        return hasBooking;
    }

    public async Task<List<CarAvailabilityBlock>> GetOverlappingBlocksAsync(
        int carId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.CarAvailabilityBlocks
            .Where(b => b.CarId == carId && b.StartDateTime < end && b.EndDateTime > start)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetOverlappingBookingsAsync(
        int carId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Bookings
            .Where(b => b.CarId == carId
                       && b.Status != BookingStatus.Cancelled
                       && b.Status != BookingStatus.Rejected
                       && b.Status != BookingStatus.Expired
                       && b.StartDateTime < end
                       && b.EndDateTime > start)
            .ToListAsync(cancellationToken);
    }

    public async Task<Voucher?> GetVoucherByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpper();
        return await dbContext.Vouchers
            .SingleOrDefaultAsync(v => v.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<Car?> GetCarByIdAsync(int carId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Cars
            .Include(c => c.Owner)
            .SingleOrDefaultAsync(c => c.Id == carId, cancellationToken);
    }

    public async Task<DriverDocument?> GetDriverDocumentByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.DriverDocuments
            .SingleOrDefaultAsync(d => d.UserId == userId, cancellationToken);
    }

    public async Task AddDriverDocumentAsync(DriverDocument doc, CancellationToken cancellationToken = default)
    {
        await dbContext.DriverDocuments.AddAsync(doc, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IDbContextTransaction? BeginTransaction()
    {
        return dbContext.Database.CurrentTransaction ?? dbContext.Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Database.CurrentTransaction ?? await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }
}
