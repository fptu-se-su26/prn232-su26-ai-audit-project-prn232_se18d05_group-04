using System.Text.Json;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Interfaces;
using Services.Models.Admin;

namespace Services.Implementations;

public class AdminCarService(ICarRepository carRepository) : IAdminCarService
{
    private static readonly JsonSerializerOptions AuditJsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<PagedResult<AdminCarResponse>> GetCarsAsync(
        AdminCarListQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var criteria = new AdminCarSearchCriteria
        {
            Page = page,
            PageSize = pageSize,
            Keyword = query.Keyword,
            Status = query.Status,
            TypeId = query.TypeId,
            FuelType = query.FuelType,
            Transmission = query.Transmission
        };

        var (items, totalItems) = await carRepository.GetAdminCarsAsync(criteria, cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResult<AdminCarResponse>(items.Select(MapCar).ToList(), page, pageSize, totalItems, totalPages);
    }

    public async Task<AdminCarResponse?> GetCarAsync(int carId, CancellationToken cancellationToken = default)
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return car is null ? null : MapCar(car);
    }

    public async Task<AdminCarResponse> CreateCarAsync(
        int adminUserId,
        AdminCarUpsertRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateRequestAsync(request, null, cancellationToken);
        var model = await ResolveCarModelAsync(request, cancellationToken);
        var carType = await ResolveCarTypeAsync(request.TypeId, cancellationToken);

        var car = new Car
        {
            OwnerId = request.OwnerId,
            CarBrandId = model.CarBrandId,
            CarModelId = model.Id,
            CarTypeId = carType?.Id,
            Name = BuildCarName(model.CarBrand.Name, model.Name, request.Year),
            LicensePlate = request.LicensePlate.Trim().ToUpperInvariant(),
            Year = request.Year.HasValue ? (short)request.Year.Value : null,
            Color = NormalizeOptional(request.Color),
            KilometersDriven = request.KilometersDriven ?? 0,
            Description = NormalizeOptional(request.Description),
            Location = request.Address.Trim(),
            DailyPrice = request.PricePerDay,
            PricePerHour = request.PricePerHours,
            InsuranceFeePerDay = 0,
            DeliveryFee = 0,
            DepositAmount = 0,
            Status = ParseStatus(request.Status),
            PreviousStatus = null,
            BlockedReason = null,
            SeatCount = request.Seats,
            TransmissionType = ParseTransmission(request.Transmission),
            FuelType = ParseFuelType(request.FuelType),
            CreatedAt = DateTime.UtcNow
        };

        await carRepository.AddCarAsync(car, cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);
        await AddAuditAsync(adminUserId, "CAR_CREATED", car.Id, null, Snapshot(car), cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);

        var created = await carRepository.FindByIdWithDetailsAsync(car.Id, cancellationToken);
        return MapCar(created ?? car);
    }

    public async Task<AdminCarResponse?> UpdateCarAsync(
        int adminUserId,
        int carId,
        AdminCarUpsertRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        await ValidateRequestAsync(request, carId, cancellationToken);
        var oldValues = Snapshot(car);
        var model = await ResolveCarModelAsync(request, cancellationToken);
        var carType = await ResolveCarTypeAsync(request.TypeId, cancellationToken);

        car.OwnerId = request.OwnerId;
        car.CarBrandId = model.CarBrandId;
        car.CarModelId = model.Id;
        car.CarTypeId = carType?.Id;
        car.Name = BuildCarName(model.CarBrand.Name, model.Name, request.Year);
        car.LicensePlate = request.LicensePlate.Trim().ToUpperInvariant();
        car.Year = request.Year.HasValue ? (short)request.Year.Value : null;
        car.Color = NormalizeOptional(request.Color);
        car.KilometersDriven = request.KilometersDriven ?? 0;
        car.Description = NormalizeOptional(request.Description);
        car.Location = request.Address.Trim();
        car.DailyPrice = request.PricePerDay;
        car.PricePerHour = request.PricePerHours;
        car.Status = ParseStatus(request.Status);
        if (car.Status != CarStatus.Blocked)
        {
            car.BlockedReason = null;
            car.PreviousStatus = null;
        }
        car.SeatCount = request.Seats;
        car.TransmissionType = ParseTransmission(request.Transmission);
        car.FuelType = ParseFuelType(request.FuelType);
        car.UpdatedAt = DateTime.UtcNow;

        await AddAuditAsync(adminUserId, "CAR_UPDATED", car.Id, oldValues, Snapshot(car), cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);

        var updated = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return updated is null ? null : MapCar(updated);
    }

    public async Task<AdminCarResponse?> BlockCarAsync(
        int adminUserId,
        int carId,
        AdminCarBlockRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        var reason = NormalizeOptional(request.BlockedReason);
        if (reason is null)
        {
            throw ValidationError("blocked_reason", "Block reason is required.");
        }

        if (reason.Length > 500)
        {
            throw ValidationError("blocked_reason", "Block reason must be 500 characters or fewer.");
        }

        if (car.Status == CarStatus.Blocked)
        {
            throw new AdminCarServiceException(409, "This car is already blocked.");
        }

        if (await carRepository.HasActiveBookingAsync(carId, cancellationToken))
        {
            throw new AdminCarServiceException(409, "Cars with active bookings cannot be blocked.");
        }

        var oldValues = Snapshot(car);
        car.PreviousStatus = car.Status is CarStatus.Blocked ? car.PreviousStatus : car.Status;
        car.Status = CarStatus.Blocked;
        car.BlockedReason = reason;
        car.UpdatedAt = DateTime.UtcNow;

        await AddAuditAsync(adminUserId, "CAR_BLOCKED", car.Id, oldValues, Snapshot(car), cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);

        var updated = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return updated is null ? null : MapCar(updated);
    }

    public async Task<AdminCarResponse?> UnblockCarAsync(
        int adminUserId,
        int carId,
        AdminCarUnblockRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        if (car.Status != CarStatus.Blocked)
        {
            throw new AdminCarServiceException(409, "Only blocked cars can be unblocked.");
        }

        var oldValues = Snapshot(car);
        var targetStatus = string.IsNullOrWhiteSpace(request.TargetStatus)
            ? car.PreviousStatus ?? CarStatus.Available
            : ParseUnblockTargetStatus(request.TargetStatus);

        car.Status = targetStatus;
        car.PreviousStatus = null;
        car.BlockedReason = null;
        car.UpdatedAt = DateTime.UtcNow;

        await AddAuditAsync(adminUserId, "CAR_UNBLOCKED", car.Id, oldValues, Snapshot(car), cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);

        var updated = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return updated is null ? null : MapCar(updated);
    }

    public async Task<IReadOnlyList<AdminCarImageResponse>?> AddImagesAsync(
        int adminUserId,
        int carId,
        IReadOnlyList<AdminCarImageCreateRequest> images,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        if (images.Count == 0)
        {
            throw ValidationError("files", "At least one image is required.");
        }

        const int maxImageCount = 8;
        if (car.Images.Count + images.Count > maxImageCount)
        {
            throw ValidationError(
                "files",
                $"A car can have at most {maxImageCount} images."
            );
        }

        var oldValues = Snapshot(car);
        var nextDisplayOrder = car.Images.Count == 0 ? 1 : car.Images.Max(image => image.DisplayOrder) + 1;
        var shouldMarkFirstPrimary = !car.Images.Any(image => image.IsPrimary);
        var created = new List<CarImage>();

        foreach (var request in images)
        {
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                throw ValidationError("image_url", "Image URL is required.");
            }

            var image = new CarImage
            {
                CarId = carId,
                ImageUrl = request.ImageUrl.Trim(),
                IsPrimary = shouldMarkFirstPrimary || request.IsPrimary,
                DisplayOrder = nextDisplayOrder++
            };

            if (image.IsPrimary)
            {
                foreach (var currentImage in car.Images)
                {
                    currentImage.IsPrimary = false;
                }
                shouldMarkFirstPrimary = false;
            }

            await carRepository.AddImageAsync(image, cancellationToken);
            created.Add(image);
        }

        await AddAuditAsync(adminUserId, "CAR_IMAGE_UPLOADED", carId, oldValues, new { images = created.Select(MapImage).ToList() }, cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);
        return created.Select(MapImage).ToList();
    }

    public async Task<AdminCarImageResponse?> DeleteImageAsync(
        int adminUserId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        var image = car.Images.SingleOrDefault(item => item.Id == imageId);
        if (image is null) return null;

        var oldImage = MapImage(image);
        var wasPrimary = image.IsPrimary;
        carRepository.RemoveImage(image);

        if (wasPrimary)
        {
            var replacement = car.Images
                .Where(item => item.Id != imageId)
                .OrderBy(item => item.DisplayOrder)
                .FirstOrDefault();

            if (replacement is not null)
            {
                replacement.IsPrimary = true;
            }
        }

        await AddAuditAsync(adminUserId, "CAR_IMAGE_DELETED", carId, oldImage, null, cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);
        return oldImage;
    }

    public async Task<AdminCarImageResponse?> SetPrimaryImageAsync(
        int adminUserId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car is null) return null;

        var image = car.Images.SingleOrDefault(item => item.Id == imageId);
        if (image is null)
        {
            throw new AdminCarServiceException(404, "Image was not found.");
        }

        var oldValues = new { primaryImageId = car.Images.FirstOrDefault(item => item.IsPrimary)?.Id };
        foreach (var currentImage in car.Images)
        {
            currentImage.IsPrimary = currentImage.Id == imageId;
        }

        await AddAuditAsync(adminUserId, "CAR_IMAGE_PRIMARY_CHANGED", carId, oldValues, new { primaryImageId = imageId }, cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);
        return MapImage(image);
    }

    public async Task<IReadOnlyList<CarTypeResponse>> GetCarTypesAsync(CancellationToken cancellationToken = default)
    {
        var types = await carRepository.GetActiveCarTypesAsync(cancellationToken);
        return types.Select(type => new CarTypeResponse(type.Id, type.Name)).ToList();
    }

    public async Task<IReadOnlyList<CarModelResponse>> GetCarModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = await carRepository.GetActiveCarModelsAsync(cancellationToken);
        return models.Select(model => new CarModelResponse(model.Id, model.Name, model.CarBrandId, model.CarBrand.Name)).ToList();
    }

    private async Task ValidateRequestAsync(AdminCarUpsertRequest request, int? currentCarId, CancellationToken cancellationToken)
    {
        if (request.OwnerId <= 0) throw ValidationError("owner_id", "Owner is required.");
        var owner = await carRepository.FindOwnerAsync(request.OwnerId, cancellationToken);
        if (owner is null) throw ValidationError("owner_id", "Owner must reference an existing car owner.");
        if (request.CarModelId is null && string.IsNullOrWhiteSpace(request.Brand)) throw ValidationError("brand", "Brand is required.");
        if (request.CarModelId is null && string.IsNullOrWhiteSpace(request.Model)) throw ValidationError("model", "Model is required.");
        if (string.IsNullOrWhiteSpace(request.LicensePlate)) throw ValidationError("license_plate", "License plate is required.");
        if (await carRepository.LicensePlateExistsAsync(request.LicensePlate, currentCarId, cancellationToken)) throw new AdminCarServiceException(409, "License plate already exists.");
        if (request.Year is < 1900 or > 2100) throw ValidationError("year", "Year must be between 1900 and 2100.");
        if (request.Color?.Length > 50) throw ValidationError("color", "Color must be 50 characters or fewer.");
        if (request.Seats <= 0) throw ValidationError("seats", "Seats must be greater than 0.");
        if (request.KilometersDriven is < 0) throw ValidationError("kilometers_driven", "Kilometers driven cannot be negative.");
        if (request.PricePerDay <= 0) throw ValidationError("price_per_day", "Daily price must be greater than 0.");
        if (request.PricePerHours <= 0) throw ValidationError("price_per_hours", "Hourly price must be greater than 0.");
        if (string.IsNullOrWhiteSpace(request.Address)) throw ValidationError("address", "Pickup address is required.");
        _ = ParseStatus(request.Status);
        _ = ParseFuelType(request.FuelType);
        _ = ParseTransmission(request.Transmission);
        if (request.TypeId.HasValue) _ = await ResolveCarTypeAsync(request.TypeId, cancellationToken);
    }

    private async Task<CarModel> ResolveCarModelAsync(AdminCarUpsertRequest request, CancellationToken cancellationToken)
    {
        if (request.CarModelId.HasValue)
        {
            var existingModel = await carRepository.FindModelByIdAsync(request.CarModelId.Value, cancellationToken);
            if (existingModel is null) throw ValidationError("car_model_id", "Car model was not found.");
            return existingModel;
        }

        var brand = await carRepository.FindBrandByNameAsync(request.Brand, cancellationToken);
        if (brand is null)
        {
            brand = new CarBrand { Name = request.Brand.Trim(), IsActive = true };
            await carRepository.AddBrandAsync(brand, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
        }

        var model = await carRepository.FindModelByNameAsync(brand.Id, request.Model, cancellationToken);
        if (model is null)
        {
            model = new CarModel { CarBrandId = brand.Id, Name = request.Model.Trim(), IsActive = true };
            await carRepository.AddModelAsync(model, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
            model.CarBrand = brand;
        }

        return model;
    }

    private async Task<CarType?> ResolveCarTypeAsync(int? typeId, CancellationToken cancellationToken)
    {
        if (!typeId.HasValue) return null;
        var type = await carRepository.FindTypeByIdAsync(typeId.Value, cancellationToken);
        if (type is null) throw ValidationError("type_id", "Car type was not found.");
        return type;
    }

    private async Task AddAuditAsync(int adminUserId, string action, int entityId, object? oldValues, object? newValues, CancellationToken cancellationToken)
    {
        var auditLog = new AdminAuditLog
        {
            AdminUserId = adminUserId,
            Action = action,
            EntityType = "Car",
            EntityId = entityId,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues, AuditJsonOptions),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues, AuditJsonOptions),
            CreatedAtUtc = DateTime.UtcNow
        };

        await carRepository.AddAuditLogAsync(auditLog, cancellationToken);
    }

    private static object Snapshot(Car car)
    {
        return new
        {
            car.Id,
            car.OwnerId,
            car.CarBrandId,
            car.CarModelId,
            car.CarTypeId,
            car.LicensePlate,
            car.Year,
            car.Color,
            car.KilometersDriven,
            car.DailyPrice,
            car.PricePerHour,
            Status = MapStatus(car.Status),
            PreviousStatus = car.PreviousStatus.HasValue ? MapStatus(car.PreviousStatus.Value) : null,
            car.BlockedReason
        };
    }

    private static AdminCarResponse MapCar(Car car)
    {
        var images = car.Images.OrderByDescending(image => image.IsPrimary).ThenBy(image => image.DisplayOrder).Select(MapImage).ToList();
        return new AdminCarResponse(
            car.Id,
            car.OwnerId,
            car.Owner.FullName,
            car.CarBrand.Name,
            car.CarModel.Name,
            car.CarModelId,
            car.CarTypeId,
            car.CarType?.Name,
            car.LicensePlate,
            car.Year,
            car.Color,
            car.SeatCount,
            car.KilometersDriven,
            MapTransmission(car.TransmissionType),
            MapFuelType(car.FuelType),
            car.DailyPrice,
            car.PricePerHour,
            car.Location,
            car.Description,
            MapStatus(car.Status),
            car.BlockedReason,
            images.FirstOrDefault(image => image.IsPrimary)?.ImageUrl ?? images.FirstOrDefault()?.ImageUrl,
            images,
            car.CreatedAt,
            car.UpdatedAt
        );
    }

    private static AdminCarImageResponse MapImage(CarImage image) => new(image.Id, image.CarId, image.ImageUrl, image.IsPrimary, image.DisplayOrder);
    private static string BuildCarName(string brand, string model, int? year) => year.HasValue ? $"{brand} {model} {year.Value}" : $"{brand} {model}";
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static AdminCarServiceException ValidationError(string field, string message)
    {
        return new AdminCarServiceException(400, "Validation failed.", new Dictionary<string, string[]> { [field] = [message] });
    }

    private static CarStatus ParseStatus(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        null or "" or "available" => CarStatus.Available,
        "rented" => CarStatus.Rented,
        "maintenance" => CarStatus.Maintenance,
        "blocked" => CarStatus.Blocked,
        "unavailable" => CarStatus.Unavailable,
        _ => throw ValidationError("status", "Status must be available, rented, maintenance, blocked, or unavailable.")
    };

    private static CarStatus ParseUnblockTargetStatus(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        null or "" or "available" => CarStatus.Available,
        "rented" => CarStatus.Rented,
        "maintenance" => CarStatus.Maintenance,
        "unavailable" => CarStatus.Unavailable,
        _ => throw ValidationError("target_status", "Target status must be available, rented, maintenance, or unavailable.")
    };

    private static string MapStatus(CarStatus status) => status switch
    {
        CarStatus.Available => "available",
        CarStatus.Rented => "rented",
        CarStatus.Maintenance => "maintenance",
        CarStatus.Blocked => "blocked",
        CarStatus.Unavailable => "unavailable",
        _ => "maintenance"
    };

    private static FuelType ParseFuelType(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "gasoline" => FuelType.Gasoline,
        "diesel" => FuelType.Diesel,
        "electric" => FuelType.Electric,
        "hybrid" => FuelType.Hybrid,
        _ => throw ValidationError("fuel_type", "Fuel type must be gasoline, diesel, electric, or hybrid.")
    };

    private static string MapFuelType(FuelType value) => value.ToString().ToLowerInvariant();

    private static TransmissionType ParseTransmission(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "manual" => TransmissionType.Manual,
        "automatic" => TransmissionType.Automatic,
        "cvt" => TransmissionType.Cvt,
        _ => throw ValidationError("transmission", "Transmission must be manual, automatic, or cvt.")
    };

    private static string MapTransmission(TransmissionType value) => value switch
    {
        TransmissionType.Cvt => "cvt",
        _ => value.ToString().ToLowerInvariant()
    };
}
