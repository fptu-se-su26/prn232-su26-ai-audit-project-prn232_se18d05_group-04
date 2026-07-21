using BusinessObjects.Data;
using BusinessObjects.Data.Seed;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace VivuCarServer.Tests;

public class RevenueSnapshotSeedTests
{
    [Fact]
    public async Task RefreshRevenueSnapshots_RebuildsFromBookingsAndSuccessfulPayments()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<VivuCarDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new VivuCarDbContext(options);
        await CreateTablesAsync(context);
        await SeedRowsAsync(context);

        var changed = await BaseSeed.RefreshRevenueSnapshotsAsync(context);
        context.ChangeTracker.Clear();
        var snapshots = await context.DailyRevenueSnapshots
            .OrderBy(snapshot => snapshot.SnapshotDate)
            .ToListAsync();

        Assert.Equal(4, changed);
        Assert.Equal(3, snapshots.Count);
        Assert.DoesNotContain(snapshots, snapshot => snapshot.SnapshotDate == new DateOnly(2026, 6, 20));

        var june = snapshots.Single(snapshot => snapshot.SnapshotDate == new DateOnly(2026, 6, 10));
        Assert.Equal(1, june.TotalBookings);
        Assert.Equal(1, june.CompletedBookings);
        Assert.Equal(1_000_000m, june.GrossRevenue);
        Assert.Equal(300_000m, june.DepositCollected);

        var julyCancelled = snapshots.Single(snapshot => snapshot.SnapshotDate == new DateOnly(2026, 7, 6));
        Assert.Equal(1, julyCancelled.CancelledBookings);
        Assert.Equal(0m, julyCancelled.GrossRevenue);

        Assert.Equal(0, await BaseSeed.RefreshRevenueSnapshotsAsync(context));
    }

    private static Task CreateTablesAsync(VivuCarDbContext context) => context.Database.ExecuteSqlRawAsync(
        """
        CREATE TABLE Bookings (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            BookingCode TEXT NOT NULL,
            CustomerId INTEGER NOT NULL,
            CarId INTEGER NOT NULL,
            StartDateTime TEXT NOT NULL,
            EndDateTime TEXT NOT NULL,
            PickupLocation TEXT NOT NULL,
            ReturnLocation TEXT NOT NULL,
            BasePrice TEXT NOT NULL,
            InsuranceFee TEXT NOT NULL,
            DeliveryFee TEXT NOT NULL,
            DiscountAmount TEXT NOT NULL,
            DepositAmount TEXT NOT NULL,
            TotalAmount TEXT NOT NULL,
            RemainingAmount TEXT NOT NULL,
            OverdueFee TEXT NULL,
            Status TEXT NOT NULL,
            CancellationReason TEXT NULL,
            CancelledAt TEXT NULL,
            CreatedAt TEXT NOT NULL,
            UpdatedAt TEXT NULL
        );

        CREATE TABLE PaymentTransactions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            BookingId INTEGER NOT NULL,
            PaymentProvider TEXT NOT NULL,
            TransactionCode TEXT NOT NULL,
            Amount TEXT NOT NULL,
            Status TEXT NOT NULL,
            PaymentUrl TEXT NULL,
            PaidAt TEXT NULL,
            RawRequest TEXT NULL,
            RawResponse TEXT NULL,
            CreatedAt TEXT NOT NULL
        );

        CREATE TABLE DailyRevenueSnapshots (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SnapshotDate TEXT NOT NULL,
            TotalBookings INTEGER NOT NULL,
            CompletedBookings INTEGER NOT NULL,
            CancelledBookings INTEGER NOT NULL,
            GrossRevenue TEXT NOT NULL,
            NetRevenue TEXT NOT NULL,
            DepositCollected TEXT NOT NULL,
            GeneratedAt TEXT NOT NULL
        );
        """);

    private static Task SeedRowsAsync(VivuCarDbContext context) => context.Database.ExecuteSqlRawAsync(
        """
        INSERT INTO Bookings
            (BookingCode, CustomerId, CarId, StartDateTime, EndDateTime, PickupLocation, ReturnLocation,
             BasePrice, InsuranceFee, DeliveryFee, DiscountAmount, DepositAmount, TotalAmount,
             RemainingAmount, Status, CreatedAt)
        VALUES
            ('TEST-JUNE', 1, 1, '2026-06-11', '2026-06-12', 'Da Nang', 'Da Nang',
             '1000000', '0', '0', '0', '300000', '1000000', '0', 'Completed', '2026-06-10 08:00:00'),
            ('TEST-JULY', 1, 1, '2026-07-06', '2026-07-07', 'Da Nang', 'Da Nang',
             '500000', '0', '0', '0', '200000', '500000', '0', 'Completed', '2026-07-05 08:00:00'),
            ('TEST-CANCELLED', 1, 1, '2026-07-07', '2026-07-08', 'Da Nang', 'Da Nang',
             '600000', '0', '0', '0', '200000', '600000', '0', 'Cancelled', '2026-07-06 08:00:00');

        INSERT INTO PaymentTransactions
            (BookingId, PaymentProvider, TransactionCode, Amount, Status, PaidAt, CreatedAt)
        VALUES
            (1, 'VNPay', 'TEST-PAY-JUNE', '1000000', 'Success', '2026-06-10 09:00:00', '2026-06-10 09:00:00'),
            (2, 'VNPay', 'TEST-PAY-JULY', '500000', 'Success', '2026-07-05 09:00:00', '2026-07-05 09:00:00');

        INSERT INTO DailyRevenueSnapshots
            (SnapshotDate, TotalBookings, CompletedBookings, CancelledBookings,
             GrossRevenue, NetRevenue, DepositCollected, GeneratedAt)
        VALUES
            ('2026-06-20', 99, 99, 0, '99000000', '99000000', '0', '2026-06-20 23:00:00');
        """);
}