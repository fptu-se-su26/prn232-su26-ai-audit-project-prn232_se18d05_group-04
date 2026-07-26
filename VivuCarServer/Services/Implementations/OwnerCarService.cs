using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Owner;

namespace Services.Implementations;

public class OwnerCarService(ICarRepository carRepository) : IOwnerCarService
{
    public async Task<PagedResult<OwnerCarResponse>> GetOwnerCarsAsync(
        int ownerId,
        OwnerCarListQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var criteria = new Repositories.Models.AdminCarSearchCriteria
        {
            Page = page,
            PageSize = pageSize,
            Keyword = query.Search,
            Status = query.Status,
            OwnerId = ownerId
        };

        var (items, totalItems) = await carRepository.GetAdminCarsAsync(criteria, cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResult<OwnerCarResponse>
        {
            Items = items.Select(MapCarToResponse).ToList(),
            TotalCount = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OwnerCarDetailResponse?> GetOwnerCarDetailAsync(
        int ownerId,
        int carId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return null;
        return MapCarToDetailResponse(car);
    }

    public async Task<OwnerCarDetailResponse> CreateOwnerCarAsync(
        int ownerId,
        OwnerCarUpsertRequest request,
        CancellationToken cancellationToken = default
    )
    {
        // Validate license plate unique
        if (await carRepository.LicensePlateExistsAsync(request.LicensePlate, null, cancellationToken))
            throw new OwnerCarServiceException(409, "Biển số xe đã tồn tại trong hệ thống.");

        // Resolve brand/model (create if not exists)
        var brand = await carRepository.FindBrandByNameAsync(request.BrandName, cancellationToken);
        if (brand == null)
        {
            brand = new CarBrand { Name = request.BrandName.Trim(), IsActive = true };
            await carRepository.AddBrandAsync(brand, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
        }

        var model = await carRepository.FindModelByNameAsync(brand.Id, request.ModelName, cancellationToken);
        if (model == null)
        {
            model = new CarModel { CarBrandId = brand.Id, Name = request.ModelName.Trim(), IsActive = true };
            await carRepository.AddModelAsync(model, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
            model.CarBrand = brand;
        }

        CarType? carType = null;
        if (request.CarTypeId.HasValue)
            carType = await carRepository.FindTypeByIdAsync(request.CarTypeId.Value, cancellationToken);

        var car = new Car
        {
            OwnerId = ownerId,
            CarBrandId = brand.Id,
            CarModelId = model.Id,
            CarTypeId = carType?.Id,
            Name = request.Name.Trim(),
            LicensePlate = request.LicensePlate.Trim().ToUpperInvariant(),
            Year = request.Year,
            Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color.Trim(),
            KilometersDriven = request.KilometersDriven,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Location = request.Location.Trim(),
            DailyPrice = request.DailyPrice,
            PricePerHour = request.PricePerHour,
            InsuranceFeePerDay = request.InsuranceFeePerDay,
            DeliveryFee = request.DeliveryFee,
            DepositAmount = request.DepositAmount,
            Status = CarStatus.Available,
            SeatCount = request.SeatCount,
            TransmissionType = ParseTransmission(request.TransmissionType),
            FuelType = ParseFuelType(request.FuelType),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await carRepository.AddCarAsync(car, cancellationToken);
        await carRepository.SaveChangesAsync(cancellationToken);

        var created = await carRepository.FindByIdWithDetailsAsync(car.Id, cancellationToken);
        return MapCarToDetailResponse(created ?? car);
    }

    public async Task<OwnerCarDetailResponse?> UpdateOwnerCarAsync(
        int ownerId,
        int carId,
        OwnerCarUpsertRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return null;

        if (await carRepository.LicensePlateExistsAsync(request.LicensePlate, carId, cancellationToken))
            throw new OwnerCarServiceException(409, "Biển số xe đã tồn tại trong hệ thống.");

        var brand = await carRepository.FindBrandByNameAsync(request.BrandName, cancellationToken);
        if (brand == null)
        {
            brand = new CarBrand { Name = request.BrandName.Trim(), IsActive = true };
            await carRepository.AddBrandAsync(brand, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
        }

        var model = await carRepository.FindModelByNameAsync(brand.Id, request.ModelName, cancellationToken);
        if (model == null)
        {
            model = new CarModel { CarBrandId = brand.Id, Name = request.ModelName.Trim(), IsActive = true };
            await carRepository.AddModelAsync(model, cancellationToken);
            await carRepository.SaveChangesAsync(cancellationToken);
            model.CarBrand = brand;
        }

        CarType? carType = null;
        if (request.CarTypeId.HasValue)
            carType = await carRepository.FindTypeByIdAsync(request.CarTypeId.Value, cancellationToken);

        car.CarBrandId = brand.Id;
        car.CarModelId = model.Id;
        car.CarTypeId = carType?.Id;
        car.Name = request.Name.Trim();
        car.LicensePlate = request.LicensePlate.Trim().ToUpperInvariant();
        car.Year = request.Year;
        car.Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color.Trim();
        car.KilometersDriven = request.KilometersDriven;
        car.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        car.Location = request.Location.Trim();
        car.DailyPrice = request.DailyPrice;
        car.PricePerHour = request.PricePerHour;
        car.InsuranceFeePerDay = request.InsuranceFeePerDay;
        car.DeliveryFee = request.DeliveryFee;
        car.DepositAmount = request.DepositAmount;
        car.SeatCount = request.SeatCount;
        car.TransmissionType = ParseTransmission(request.TransmissionType);
        car.FuelType = ParseFuelType(request.FuelType);
        car.UpdatedAt = DateTime.UtcNow;

        await carRepository.SaveChangesAsync(cancellationToken);
        var updated = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return MapCarToDetailResponse(updated ?? car);
    }

    public async Task<OwnerCarDetailResponse?> UpdateCarStatusAsync(
        int ownerId,
        int carId,
        string status,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return null;

        var newStatus = ParseOwnerStatus(status);

        // Cannot set to Blocked (admin-only)
        if (newStatus == CarStatus.Blocked)
            throw new OwnerCarServiceException(403, "Chỉ quản trị viên mới có thể khóa xe.");

        // Cannot set to Unavailable if there's an active booking
        if (newStatus == CarStatus.Unavailable && await carRepository.HasActiveBookingAsync(carId, cancellationToken))
            throw new OwnerCarServiceException(409, "Không thể tạm ngưng xe khi đang có chuyến thuê chưa hoàn tất.");

        car.Status = newStatus;
        car.UpdatedAt = DateTime.UtcNow;
        await carRepository.SaveChangesAsync(cancellationToken);

        var updated = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        return MapCarToDetailResponse(updated ?? car);
    }

    public async Task<IReadOnlyList<OwnerCarImageResponse>?> AddCarImagesAsync(
        int ownerId,
        int carId,
        IReadOnlyList<OwnerCarImageCreateRequest> images,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return null;

        if (images.Count == 0) throw new OwnerCarServiceException(400, "Cần ít nhất một ảnh.");

        var nextOrder = car.Images.Count == 0 ? 1 : car.Images.Max(i => i.DisplayOrder) + 1;
        var shouldMarkFirstPrimary = !car.Images.Any(i => i.IsPrimary);
        var created = new List<CarImage>();

        foreach (var req in images)
        {
            if (string.IsNullOrWhiteSpace(req.ImageUrl))
                throw new OwnerCarServiceException(400, "URL ảnh không được để trống.");

            var img = new CarImage
            {
                CarId = carId,
                ImageUrl = req.ImageUrl.Trim(),
                IsPrimary = shouldMarkFirstPrimary || req.IsPrimary,
                DisplayOrder = nextOrder++
            };

            if (img.IsPrimary)
            {
                foreach (var existing in car.Images) existing.IsPrimary = false;
                shouldMarkFirstPrimary = false;
            }

            await carRepository.AddImageAsync(img, cancellationToken);
            created.Add(img);
        }

        await carRepository.SaveChangesAsync(cancellationToken);
        return created.Select(MapImage).ToList();
    }

    public async Task<bool> DeleteCarImageAsync(
        int ownerId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return false;

        var image = car.Images.SingleOrDefault(i => i.Id == imageId);
        if (image == null) return false;

        var wasPrimary = image.IsPrimary;
        carRepository.RemoveImage(image);

        if (wasPrimary)
        {
            var replacement = car.Images.Where(i => i.Id != imageId).OrderBy(i => i.DisplayOrder).FirstOrDefault();
            if (replacement != null) replacement.IsPrimary = true;
        }

        await carRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetPrimaryImageAsync(
        int ownerId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return false;

        var image = car.Images.SingleOrDefault(i => i.Id == imageId);
        if (image == null) throw new OwnerCarServiceException(404, "Không tìm thấy ảnh.");

        foreach (var img in car.Images) img.IsPrimary = (img.Id == imageId);
        await carRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<OwnerCarActivityResponse?> GetCarActivityAsync(
        int ownerId,
        int carId,
        CancellationToken cancellationToken = default
    )
    {
        var car = await carRepository.FindByIdWithDetailsAsync(carId, cancellationToken);
        if (car == null || car.OwnerId != ownerId) return null;

        var completedBookings = car.Bookings.Where(b => b.Status == BookingStatus.Completed).ToList();
        var totalRentalDays = completedBookings.Sum(b => (int)Math.Ceiling((b.EndDateTime - b.StartDateTime).TotalDays));
        var totalRevenue = completedBookings.Sum(b => b.TotalAmount);
        var avgRating = car.Reviews.Any() ? car.Reviews.Average(r => (double)r.Rating) : 0.0;

        var recentBookings = car.Bookings
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new OwnerBookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode,
                CustomerName = b.Customer?.FullName ?? "Khách",
                CustomerPhone = b.Customer?.PhoneNumber ?? "",
                CarName = car.Name,
                LicensePlate = car.LicensePlate,
                StartDateTime = b.StartDateTime,
                EndDateTime = b.EndDateTime,
                TotalAmount = b.TotalAmount,
                DepositAmount = b.DepositAmount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            }).ToList();

        return new OwnerCarActivityResponse
        {
            CarId = carId,
            TotalBookings = car.Bookings.Count,
            CompletedTrips = completedBookings.Count,
            TotalRentalDays = totalRentalDays,
            TotalRevenue = totalRevenue,
            AverageRating = Math.Round(avgRating, 1),
            RecentBookings = recentBookings
        };
    }

    public async Task<IReadOnlyList<CarTypeResponse>> GetCarTypesAsync(CancellationToken cancellationToken = default)
    {
        var types = await carRepository.GetActiveCarTypesAsync(cancellationToken);
        return types.Select(t => new CarTypeResponse { Id = t.Id, Name = t.Name }).ToList();
    }

    public async Task<IReadOnlyList<CarModelResponse>> GetCarModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = await carRepository.GetActiveCarModelsAsync(cancellationToken);
        return models.Select(m => new CarModelResponse { Id = m.Id, Name = m.Name, BrandName = m.CarBrand?.Name ?? "" }).ToList();
    }

    // ─── Mappers ──────────────────────────────────────────────────────────────

    private static OwnerCarResponse MapCarToResponse(Car car)
    {
        var primaryImage = car.Images.FirstOrDefault(i => i.IsPrimary) ?? car.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();
        return new OwnerCarResponse
        {
            Id = car.Id,
            Name = car.Name,
            LicensePlate = car.LicensePlate,
            Brand = car.CarBrand?.Name ?? "",
            Model = car.CarModel?.Name ?? "",
            Year = car.Year,
            DailyPrice = car.DailyPrice,
            Status = car.Status.ToString().ToLower(),
            Location = car.Location,
            PrimaryImageUrl = primaryImage?.ImageUrl,
            TotalBookings = car.Bookings?.Count ?? 0,
            AverageRating = car.Reviews?.Any() == true ? Math.Round(car.Reviews.Average(r => (double)r.Rating), 1) : 0.0,
            CreatedAt = car.CreatedAt
        };
    }

    private static OwnerCarDetailResponse MapCarToDetailResponse(Car car)
    {
        var primaryImage = car.Images.FirstOrDefault(i => i.IsPrimary) ?? car.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();
        var images = car.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder).Select(MapImage).ToList();

        return new OwnerCarDetailResponse
        {
            Id = car.Id,
            Name = car.Name,
            LicensePlate = car.LicensePlate,
            Brand = car.CarBrand?.Name ?? "",
            Model = car.CarModel?.Name ?? "",
            Year = car.Year,
            Color = car.Color,
            KilometersDriven = car.KilometersDriven,
            Description = car.Description,
            DailyPrice = car.DailyPrice,
            PricePerHour = car.PricePerHour,
            InsuranceFeePerDay = car.InsuranceFeePerDay,
            DeliveryFee = car.DeliveryFee,
            DepositAmount = car.DepositAmount,
            Status = car.Status.ToString().ToLower(),
            Location = car.Location,
            SeatCount = car.SeatCount,
            TransmissionType = car.TransmissionType.ToString().ToLower(),
            FuelType = car.FuelType.ToString().ToLower(),
            CarTypeId = car.CarTypeId,
            CarType = car.CarType?.Name,
            CarBrandId = car.CarBrandId,
            CarModelId = car.CarModelId,
            PrimaryImageUrl = primaryImage?.ImageUrl,
            TotalBookings = car.Bookings?.Count ?? 0,
            AverageRating = car.Reviews?.Any() == true ? Math.Round(car.Reviews.Average(r => (double)r.Rating), 1) : 0.0,
            CreatedAt = car.CreatedAt,
            Images = images
        };
    }

    private static OwnerCarImageResponse MapImage(CarImage img) => new()
    {
        Id = img.Id,
        ImageUrl = img.ImageUrl,
        IsPrimary = img.IsPrimary,
        DisplayOrder = img.DisplayOrder
    };

    // ─── Enum Parsers ─────────────────────────────────────────────────────────

    private static CarStatus ParseOwnerStatus(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "available" => CarStatus.Available,
        "maintenance" => CarStatus.Maintenance,
        "unavailable" => CarStatus.Unavailable,
        "blocked" => CarStatus.Blocked,
        _ => throw new OwnerCarServiceException(400, "Trạng thái không hợp lệ. Chọn: available, maintenance, unavailable.")
    };

    private static FuelType ParseFuelType(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "gasoline" => FuelType.Gasoline,
        "diesel" => FuelType.Diesel,
        "electric" => FuelType.Electric,
        "hybrid" => FuelType.Hybrid,
        _ => FuelType.Gasoline
    };

    private static TransmissionType ParseTransmission(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "manual" => TransmissionType.Manual,
        "cvt" => TransmissionType.Cvt,
        _ => TransmissionType.Automatic
    };
}
