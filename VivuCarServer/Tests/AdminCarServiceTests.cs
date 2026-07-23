using BusinessObjects.Enums;
using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;

namespace VivuCarServer.Tests;

public class AdminCarServiceTests
{
    private readonly Mock<ICarRepository> repository = new();
    private readonly AdminCarService service;

    public AdminCarServiceTests()
    {
        service = new AdminCarService(repository.Object);
    }

    [Fact]
    public async Task CreateCarAsync_PersistsPhase03FieldsAndAuditLog()
    {
        var brand = new CarBrand { Id = 1, Name = "Toyota", IsActive = true };
        var model = new CarModel { Id = 2, CarBrandId = 1, Name = "Vios", CarBrand = brand, IsActive = true };
        var type = new CarType { Id = 3, Name = "Sedan", IsActive = true };
        Car? capturedCar = null;

        repository.Setup(repo => repo.FindOwnerAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser { Id = 9, Role = UserRole.CarOwner, FullName = "Owner" });
        repository.Setup(repo => repo.LicensePlateExistsAsync("43A-12345", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(repo => repo.FindModelByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        repository.Setup(repo => repo.FindTypeByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(type);
        repository.Setup(repo => repo.AddCarAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
            .Callback<Car, CancellationToken>((car, _) =>
            {
                car.Id = 77;
                car.CarBrand = brand;
                car.CarModel = model;
                car.CarType = type;
                car.Owner = new AppUser { Id = 9, FullName = "Owner" };
                capturedCar = car;
            })
            .Returns(Task.CompletedTask);
        repository.Setup(repo => repo.FindByIdWithDetailsAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => capturedCar);
        repository.Setup(repo => repo.AddAuditLogAsync(It.IsAny<AdminAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new AdminCarUpsertRequest
        {
            OwnerId = 9,
            CarModelId = 2,
            TypeId = 3,
            Brand = "Toyota",
            Model = "Vios",
            LicensePlate = "43A-12345",
            Year = 2022,
            Color = "White",
            Seats = 5,
            KilometersDriven = 28000,
            Transmission = "cvt",
            FuelType = "gasoline",
            PricePerDay = 600000m,
            PricePerHours = 90000m,
            Address = "Hai Chau, Da Nang",
            Status = "available"
        };

        var response = await service.CreateCarAsync(1, request);

        Assert.NotNull(capturedCar);
        Assert.Equal((short)2022, capturedCar.Year);
        Assert.Equal("White", capturedCar.Color);
        Assert.Equal(28000, capturedCar.KilometersDriven);
        Assert.Equal(90000m, capturedCar.PricePerHour);
        Assert.Equal(3, capturedCar.CarTypeId);
        Assert.Equal(TransmissionType.Cvt, capturedCar.TransmissionType);
        Assert.Equal("cvt", response.Transmission);
        repository.Verify(repo => repo.AddAuditLogAsync(
            It.Is<AdminAuditLog>(log => log.AdminUserId == 1 && log.Action == "CAR_CREATED" && log.EntityId == 77),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BlockCarAsync_SavesBlockedReasonPreviousStatusAndAuditLog()
    {
        var car = CreateCar(CarStatus.Maintenance);
        repository.Setup(repo => repo.FindByIdWithDetailsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);
        repository.Setup(repo => repo.HasActiveBookingAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(repo => repo.AddAuditLogAsync(It.IsAny<AdminAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await service.BlockCarAsync(12, 5, new AdminCarBlockRequest { BlockedReason = "Policy issue" });

        Assert.NotNull(response);
        Assert.Equal(CarStatus.Blocked, car.Status);
        Assert.Equal(CarStatus.Maintenance, car.PreviousStatus);
        Assert.Equal("Policy issue", car.BlockedReason);
        Assert.Equal("blocked", response.Status);
        repository.Verify(repo => repo.AddAuditLogAsync(
            It.Is<AdminAuditLog>(log => log.AdminUserId == 12 && log.Action == "CAR_BLOCKED" && log.EntityId == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnblockCarAsync_ClearsReasonAndUsesRequestedTargetStatus()
    {
        var car = CreateCar(CarStatus.Blocked);
        car.PreviousStatus = CarStatus.Maintenance;
        car.BlockedReason = "Policy issue";
        repository.Setup(repo => repo.FindByIdWithDetailsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);
        repository.Setup(repo => repo.AddAuditLogAsync(It.IsAny<AdminAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await service.UnblockCarAsync(12, 5, new AdminCarUnblockRequest { TargetStatus = "available" });

        Assert.NotNull(response);
        Assert.Equal(CarStatus.Available, car.Status);
        Assert.Null(car.PreviousStatus);
        Assert.Null(car.BlockedReason);
        Assert.Equal("available", response.Status);
        repository.Verify(repo => repo.AddAuditLogAsync(
            It.Is<AdminAuditLog>(log => log.AdminUserId == 12 && log.Action == "CAR_UNBLOCKED" && log.EntityId == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BlockCarAsync_RejectsEmptyReason()
    {
        var car = CreateCar(CarStatus.Available);
        repository.Setup(repo => repo.FindByIdWithDetailsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var exception = await Assert.ThrowsAsync<AdminCarServiceException>(() =>
            service.BlockCarAsync(12, 5, new AdminCarBlockRequest { BlockedReason = " " }));

        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("blocked_reason", exception.Errors!.Keys);
    }

    [Fact]
    public async Task AddImagesAsync_RejectsMoreThanEightTotalImages()
    {
        var car = CreateCar(CarStatus.Available);
        car.Images = Enumerable.Range(1, 8)
            .Select(index => new CarImage
            {
                Id = index,
                CarId = car.Id,
                ImageUrl = $"https://example.test/{index}.jpg",
                IsPrimary = index == 1,
                DisplayOrder = index
            })
            .ToList();
        repository.Setup(repo => repo.FindByIdWithDetailsAsync(car.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var exception = await Assert.ThrowsAsync<AdminCarServiceException>(() =>
            service.AddImagesAsync(12, car.Id, [
                new AdminCarImageCreateRequest
                {
                    ImageUrl = "https://example.test/9.jpg"
                }
            ]));

        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("files", exception.Errors!.Keys);
        repository.Verify(
            repo => repo.AddImageAsync(It.IsAny<CarImage>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
    private static Car CreateCar(CarStatus status)
    {
        var owner = new AppUser { Id = 2, FullName = "Owner", Role = UserRole.CarOwner };
        var brand = new CarBrand { Id = 1, Name = "Toyota" };
        var model = new CarModel { Id = 1, Name = "Vios", CarBrandId = 1, CarBrand = brand };
        var type = new CarType { Id = 1, Name = "Sedan" };
        return new Car
        {
            Id = 5,
            OwnerId = owner.Id,
            Owner = owner,
            CarBrandId = brand.Id,
            CarBrand = brand,
            CarModelId = model.Id,
            CarModel = model,
            CarTypeId = type.Id,
            CarType = type,
            Name = "Toyota Vios 2022",
            LicensePlate = "43A-12345",
            Year = 2022,
            Color = "White",
            KilometersDriven = 12000,
            Location = "Hai Chau, Da Nang",
            DailyPrice = 600000m,
            PricePerHour = 90000m,
            SeatCount = 5,
            TransmissionType = TransmissionType.Automatic,
            FuelType = FuelType.Gasoline,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            Images = []
        };
    }
}
