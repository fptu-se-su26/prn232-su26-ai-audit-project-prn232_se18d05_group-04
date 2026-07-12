using BusinessObjects.Enums;
using BusinessObjects.Models;
using Moq;
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
        Assert.NotNull(result); Assert.Equal(25m, result.UsageRate); Assert.Equal(900000m, result.GrossRevenue); Assert.Equal(100000m, result.DiscountTotal);
    }
}
