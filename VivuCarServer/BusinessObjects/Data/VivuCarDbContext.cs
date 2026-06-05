using BusinessObjects.Data.Seed;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data;

public class VivuCarDbContext(DbContextOptions<VivuCarDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<DriverDocument> DriverDocuments => Set<DriverDocument>();
    public DbSet<CarBrand> CarBrands => Set<CarBrand>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarImage> CarImages => Set<CarImage>();
    public DbSet<CarAvailabilityBlock> CarAvailabilityBlocks => Set<CarAvailabilityBlock>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingDriverInfo> BookingDriverInfos => Set<BookingDriverInfo>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<BookingVoucher> BookingVouchers => Set<BookingVoucher>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<RentalContract> RentalContracts => Set<RentalContract>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<IncidentReport> IncidentReports => Set<IncidentReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VivuCarDbContext).Assembly);
        modelBuilder.SeedData();
    }
}
