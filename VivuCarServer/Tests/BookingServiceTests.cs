using BusinessObjects.Enums;
using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Models.Booking;
using Xunit;

namespace VivuCarServer.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepo;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockRepo = new Mock<IBookingRepository>();
        _bookingService = new BookingService(_mockRepo.Object);
    }

    [Fact]
    public async Task CheckAvailability_WhenNoOverlap_ReturnsTrue()
    {
        _mockRepo.Setup(r => r.CheckOverlapExistsAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _bookingService.CheckAvailabilityAsync(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.True(result);
    }

    [Fact]
    public async Task CheckAvailability_WhenOverlapExists_ReturnsFalse()
    {
        _mockRepo.Setup(r => r.CheckOverlapExistsAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _bookingService.CheckAvailabilityAsync(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result);
    }

    [Fact]
    public async Task CalculatePricePreview_CalculatesCorrectWeekdayAndWeekendPrices()
    {
        var car = new Car
        {
            Id = 1,
            DailyPrice = 500000m,
            InsuranceFeePerDay = 50000m,
            DeliveryFee = 10000m,
            DepositAmount = 0m // defaults to 30%
        };

        _mockRepo.Setup(r => r.GetCarByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        // Monday to Wednesday (2 days - both weekdays)
        var start = new DateTime(2026, 6, 15, 10, 0, 0); // Monday
        var end = new DateTime(2026, 6, 17, 10, 0, 0);   // Wednesday

        var request = new PricePreviewRequest
        {
            CarId = 1,
            StartDateTime = start,
            EndDateTime = end,
            HasInsurance = true,
            HasDelivery = true,
            DistanceKm = 10m
        };

        var preview = await _bookingService.CalculatePricePreviewAsync(request);

        // Weekday count: 2, Weekend count: 0
        Assert.Equal(2, preview.RentalDays);
        Assert.Equal(2, preview.WeekdayCount);
        Assert.Equal(0, preview.WeekendCount);
        Assert.Equal(1000000m, preview.WeekdayCost); // 500k * 2
        Assert.Equal(100000m, preview.InsuranceFee); // 50k * 2
        Assert.Equal(100000m, preview.DeliveryFee);  // 10k * 10km = 100k
        Assert.Equal(1200000m, preview.TotalAmount); // 1M + 100k + 100k
        Assert.Equal(360000m, preview.DepositAmount); // 30% of 1.2M
    }

    [Fact]
    public async Task CreateBooking_Throws_WhenOwnerBooksOwnCar()
    {
        var car = new Car { Id = 1, OwnerId = 99 };
        _mockRepo.Setup(r => r.GetCarByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var request = new CreateBookingRequest
        {
            CarId = 1,
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _bookingService.CreateBookingAsync(99, request));
    }
}
