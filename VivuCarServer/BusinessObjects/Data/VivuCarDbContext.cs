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
    public DbSet<CarType> CarTypes => Set<CarType>();
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
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();
    public DbSet<DailyRevenueSnapshot> DailyRevenueSnapshots => Set<DailyRevenueSnapshot>();
    public DbSet<ExportJob> ExportJobs => Set<ExportJob>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

    public async Task<SeedSummary> SeedAsync(
        UserSeedOptions userOptions,
        Func<AppUser, string> passwordHashFactory,
        CancellationToken cancellationToken = default
    )
    {
        var usersAdded = await UserSeed.SeedAsync(
            this,
            userOptions,
            passwordHashFactory,
            cancellationToken
        );
        var carResult = await CarSeed.SeedAsync(this, cancellationToken);
        var baseResult = await BaseSeed.SeedAsync(this, carResult.Cars, cancellationToken);

        return new SeedSummary(
            usersAdded,
            carResult.CarsAdded,
            carResult.ImagesAdded,
            baseResult.BookingsAdded,
            baseResult.ReviewsAdded,
            baseResult.PaymentsAdded,
            baseResult.RevenueSnapshotsAdded
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VivuCarDbContext).Assembly);

        // Explicit precision for nullable decimal columns added in later migrations
        modelBuilder.Entity<Booking>()
            .Property(b => b.OverdueFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(b => b.ExtraFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<IncidentReport>()
            .Property(i => i.PenaltyAmount)
            .HasPrecision(18, 2);
    }
}