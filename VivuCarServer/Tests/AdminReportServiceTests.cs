using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Implementations;

namespace VivuCarServer.Tests;

public class AdminReportServiceTests
{
    private readonly Mock<IAdminReportRepository> repository = new();
    private readonly AdminReportService service;

    public AdminReportServiceTests()
    {
        repository.Setup(item => item.GetRevenueStatusByDateAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        repository.Setup(item => item.GetRevenueByHourAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        service = new AdminReportService(repository.Object);
    }

    [Fact]
    public async Task GetRevenueAsync_MapsSnapshotsAndCalculatesSummary()
    {
        var from = new DateOnly(2026, 7, 1);
        var to = new DateOnly(2026, 7, 2);
        repository.Setup(item => item.GetRevenueSnapshotsAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyRevenueSnapshot>
            {
                new() { SnapshotDate = from, TotalBookings = 5, CompletedBookings = 3, CancelledBookings = 1, GrossRevenue = 3_600_000m, NetRevenue = 3_100_000m, DepositCollected = 1_080_000m },
                new() { SnapshotDate = to, TotalBookings = 4, CompletedBookings = 2, CancelledBookings = 1, GrossRevenue = 2_400_000m, NetRevenue = 2_000_000m, DepositCollected = 720_000m }
            });
        repository.Setup(item => item.CountBookingsAsync(from, to, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        repository.Setup(item => item.GetRevenueStatusByDateAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminRevenueStatusRecord>
            {
                new() { Date = from, PendingPaymentAmount = 1_400_000m, CancelledAmount = 650_000m, PaidBookings = 3, PendingPaymentBookings = 1, CancelledWithoutPaymentBookings = 1 },
                new() { Date = to, PendingPaymentAmount = 900_000m, CancelledAmount = 400_000m, PaidBookings = 2, PendingPaymentBookings = 1, CancelledWithoutPaymentBookings = 1 }
            });
        repository.Setup(item => item.GetBookingsPageAsync(from, to, 0, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminRecentBookingRecord>
            {
                new() { Id = 8, BookingCode = "VC-260708", CustomerName = "Nguyen Minh Anh", CarName = "Toyota Vios", PickupDate = DateTime.UtcNow, TotalAmount = 1_200_000m, BookingStatus = "Completed", PaymentStatus = "Success" }
            });

        var result = await service.GetRevenueAsync(from, to);

        Assert.Equal(6_000_000m, result.Summary.GrossRevenue);
        Assert.Equal(5_100_000m, result.Summary.NetRevenue);
        Assert.Equal(1_800_000m, result.Summary.DepositCollected);
        Assert.Equal(1_400_000m, result.Daily[0].PendingPaymentAmount);
        Assert.Equal(650_000m, result.Daily[0].CancelledAmount);
        Assert.Equal(5, result.Summary.PaidBookings);
        Assert.Equal(2, result.Summary.PendingPaymentBookings);
        Assert.Equal(9, result.Summary.TotalBookings);
        Assert.Equal(5, result.Summary.CompletedBookings);
        Assert.Equal(1_200_000m, result.Summary.AverageOrderValue);
        Assert.Equal(22.22m, result.Summary.CancelRate);
        Assert.Equal("completed", result.RecentBookings[0].BookingStatus);
        Assert.Equal("success", result.RecentBookings[0].PaymentStatus);
        Assert.Equal(1, result.Pagination.TotalItems);
        Assert.Equal(1, result.Pagination.TotalPages);
    }

    [Fact]
    public async Task GetRevenueAsync_ForSingleDay_ReturnsAllTwentyFourHours()
    {
        var date = new DateOnly(2026, 7, 25);
        repository.Setup(item => item.GetRevenueSnapshotsAsync(date, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        repository.Setup(item => item.GetRevenueByHourAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminHourlyRevenueRecord>
            {
                new() { Hour = 9, GrossRevenue = 1_250_000m, PaidTransactions = 1 },
                new() { Hour = 17, GrossRevenue = 2_400_000m, PaidTransactions = 2 }
            });
        repository.Setup(item => item.CountBookingsAsync(date, date, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        repository.Setup(item => item.GetBookingsPageAsync(date, date, 0, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await service.GetRevenueAsync(date, date);

        Assert.Equal(24, result.Hourly.Count);
        Assert.Equal(Enumerable.Range(0, 24), result.Hourly.Select(item => item.Hour));
        Assert.Equal(1_250_000m, result.Hourly[9].GrossRevenue);
        Assert.Equal(2, result.Hourly[17].PaidTransactions);
        Assert.Equal(0, result.Hourly[10].GrossRevenue);
    }
    [Fact]
    public async Task GetRevenueAsync_WhenSnapshotsAreEmpty_ReturnsZeroSummary()
    {
        var from = new DateOnly(2026, 7, 1);
        var to = new DateOnly(2026, 7, 31);
        repository.Setup(item => item.GetRevenueSnapshotsAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        repository.Setup(item => item.CountBookingsAsync(from, to, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        repository.Setup(item => item.GetBookingsPageAsync(from, to, 0, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await service.GetRevenueAsync(from, to);

        Assert.Equal(0, result.Summary.GrossRevenue);
        Assert.Equal(0, result.Summary.AverageOrderValue);
        Assert.Equal(0, result.Summary.CancelRate);
        Assert.Empty(result.Daily);
        Assert.Equal(0, result.Pagination.TotalItems);
    }

    [Fact]
    public async Task GetRevenueAsync_WaitsForSnapshotsBeforeQueryingBookings()
    {
        var from = new DateOnly(2026, 7, 1);
        var to = new DateOnly(2026, 7, 31);
        var snapshotsCompletion = new TaskCompletionSource<IReadOnlyList<DailyRevenueSnapshot>>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var bookingQueryStarted = false;

        repository.Setup(item => item.GetRevenueSnapshotsAsync(from, to, It.IsAny<CancellationToken>()))
            .Returns(snapshotsCompletion.Task);
        repository.Setup(item => item.CountBookingsAsync(from, to, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback(() => bookingQueryStarted = true)
            .ReturnsAsync(0);
        repository.Setup(item => item.GetBookingsPageAsync(from, to, 0, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var reportTask = service.GetRevenueAsync(from, to);
        await Task.Yield();

        Assert.False(bookingQueryStarted);

        snapshotsCompletion.SetResult([]);
        await reportTask;

        Assert.True(bookingQueryStarted);
    }

    [Fact]
    public async Task GetRevenueAsync_UsesRequestedBookingPage()
    {
        var from = new DateOnly(2026, 7, 1);
        var to = new DateOnly(2026, 7, 31);
        repository.Setup(item => item.GetRevenueSnapshotsAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        repository.Setup(item => item.CountBookingsAsync(from, to, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(12);
        repository.Setup(item => item.GetBookingsPageAsync(from, to, 5, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await service.GetRevenueAsync(from, to, page: 2, pageSize: 5);

        Assert.Equal(2, result.Pagination.Page);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(12, result.Pagination.TotalItems);
        Assert.Equal(3, result.Pagination.TotalPages);
        repository.Verify(item => item.GetBookingsPageAsync(from, to, 5, 5, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRevenueAsync_WhenFromAfterTo_ThrowsValidationError()
    {
        await Assert.ThrowsAsync<AdminReportValidationException>(() =>
            service.GetRevenueAsync(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 1)));
    }

    [Fact]
    public async Task GetRevenueAsync_WhenRangeExceeds366Days_ThrowsValidationError()
    {
        await Assert.ThrowsAsync<AdminReportValidationException>(() =>
            service.GetRevenueAsync(new DateOnly(2025, 1, 1), new DateOnly(2026, 1, 2)));
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 0)]
    [InlineData(1, 51)]
    public async Task GetRevenueAsync_WhenPaginationIsInvalid_ThrowsValidationError(int page, int pageSize)
    {
        await Assert.ThrowsAsync<AdminReportValidationException>(() =>
            service.GetRevenueAsync(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 2), page, pageSize));
    }
}