using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;

namespace VivuCarServer.Tests;

public class AdminVoucherServiceTests
{
    private readonly Mock<IAdminVoucherRepository> repository = new();
    private AdminVoucherService Service() => new(repository.Object);
    private static AdminVoucherUpsertRequest ValidRequest() => new()
    { Name = "Summer", Code = " summer-10 ", DiscountType = "percentage", DiscountValue = 10, MinOrderAmount = 500000, MaxDiscount = 200000, Quantity = 20 };

    [Fact]
    public async Task CreateAsync_NormalizesCodeAndPersistsVoucher()
    {
        Voucher? captured = null;
        repository.Setup(x => x.CodeExistsAsync("SUMMER-10", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        repository.Setup(x => x.AddAsync(It.IsAny<Voucher>(), It.IsAny<CancellationToken>())).Callback<Voucher, CancellationToken>((value, _) => { value.Id = 8; captured = value; }).Returns(Task.CompletedTask);
        repository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var result = await Service().CreateAsync(ValidRequest());
        Assert.Equal("SUMMER-10", result.Code); Assert.Equal(8, result.Id); Assert.Equal(DiscountType.Percentage, captured!.DiscountType);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateCode()
    {
        repository.Setup(x => x.CodeExistsAsync("SUMMER-10", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var exception = await Assert.ThrowsAsync<AdminVoucherServiceException>(() => Service().CreateAsync(ValidRequest()));
        Assert.Equal(400, exception.StatusCode); Assert.Contains("code", exception.Errors!.Keys);
    }

    [Fact]
    public async Task DeleteAsync_RejectsVoucherWithUsage()
    {
        var voucher = new Voucher { Id = 3, Name = "Used", Code = "USED", Quantity = 1, BookingVouchers = [new BookingVoucher()] };
        repository.Setup(x => x.GetByIdAsync(3, true, It.IsAny<CancellationToken>())).ReturnsAsync(voucher);
        var exception = await Assert.ThrowsAsync<AdminVoucherServiceException>(() => Service().DeleteAsync(3));
        Assert.Equal(409, exception.StatusCode); repository.Verify(x => x.Remove(It.IsAny<Voucher>()), Times.Never);
    }

    [Fact]
    public async Task GetPerformanceAsync_UsesRecordedDiscountAndBookingRevenue()
    {
        var customer = new AppUser { FullName = "Customer" };
        var booking = new Booking { Id = 5, BookingCode = "BK-5", Customer = customer, TotalAmount = 900000 };
        var voucher = new Voucher { Id = 2, Name = "Test", Code = "TEST", DiscountType = DiscountType.Fixed, DiscountValue = 100000, MaxDiscount = 100000, Quantity = 4, BookingVouchers = [new BookingVoucher { BookingId = 5, Booking = booking, DiscountAmount = 100000, AppliedAt = DateTime.UtcNow }] };
        repository.Setup(x => x.GetByIdAsync(2, true, It.IsAny<CancellationToken>())).ReturnsAsync(voucher);
        var result = await Service().GetPerformanceAsync(2);
        Assert.NotNull(result);
        Assert.Equal(25m, result.UsageRate);
        Assert.Equal(900000m, result.GrossRevenue);
        Assert.Equal(100000m, result.DiscountTotal);
        Assert.Equal(900000m, result.RecentUsages.Single().OrderAmount);
    }

    [Fact]
    public async Task CreateAsync_RejectsPercentageAboveOneHundred()
    {
        var request = ValidRequest();
        request.DiscountValue = 101;

        var exception = await Assert.ThrowsAsync<AdminVoucherServiceException>(
            () => Service().CreateAsync(request));

        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("discount_value", exception.Errors!.Keys);
    }

    [Fact]
    public async Task UpdateAsync_RejectsQuantityBelowRecordedUsage()
    {
        var voucher = new Voucher
        {
            Id = 4,
            Name = "Used twice",
            Code = "USED-TWICE",
            Quantity = 3,
            BookingVouchers = [new BookingVoucher(), new BookingVoucher()]
        };
        var request = ValidRequest();
        request.Quantity = 1;
        repository.Setup(x => x.GetByIdAsync(4, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voucher);
        repository.Setup(x => x.CodeExistsAsync("SUMMER-10", 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<AdminVoucherServiceException>(
            () => Service().UpdateAsync(4, request));

        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("quantity", exception.Errors!.Keys);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetListAsync_FiltersExpiryByInclusiveDateRange()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<VivuCarDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new VivuCarDbContext(options);
        await dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE "Vouchers" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Vouchers" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL,
                "Code" TEXT NOT NULL,
                "DiscountType" TEXT NOT NULL,
                "DiscountValue" TEXT NOT NULL,
                "MinOrderAmount" TEXT NOT NULL DEFAULT '0.0',
                "MaxDiscount" TEXT NOT NULL,
                "Quantity" INTEGER NOT NULL,
                "ExpiresAt" TEXT NULL,
                "CreatedAt" TEXT NOT NULL
            );
            """);
        await dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE "BookingVouchers" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_BookingVouchers" PRIMARY KEY AUTOINCREMENT,
                "BookingId" INTEGER NOT NULL,
                "VoucherId" INTEGER NOT NULL,
                "Code" TEXT NOT NULL,
                "DiscountAmount" TEXT NOT NULL,
                "AppliedAt" TEXT NOT NULL
            );
            """);
        dbContext.Vouchers.AddRange(
            VoucherExpiring("JULY", new DateTime(2026, 7, 31, 23, 59, 0, DateTimeKind.Utc)),
            VoucherExpiring("AUGUST-START", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)),
            VoucherExpiring("AUGUST-END", new DateTime(2026, 8, 31, 23, 59, 0, DateTimeKind.Utc)),
            VoucherExpiring("SEPTEMBER", new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)));
        await dbContext.SaveChangesAsync();

        var service = new AdminVoucherService(new AdminVoucherRepository(dbContext));
        var result = await service.GetListAsync(new AdminVoucherListQuery
        {
            Page = 1,
            PageSize = 10,
            From = new DateOnly(2026, 8, 1),
            To = new DateOnly(2026, 8, 31)
        });

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(["AUGUST-END", "AUGUST-START"], result.Items.Select(item => item.Code));
    }

    private static Voucher VoucherExpiring(string code, DateTime expiresAt) => new()
    {
        Name = code,
        Code = code,
        DiscountType = DiscountType.Fixed,
        DiscountValue = 50000,
        MaxDiscount = 50000,
        Quantity = 10,
        ExpiresAt = expiresAt,
        CreatedAt = expiresAt.AddDays(-10)
    };
}
