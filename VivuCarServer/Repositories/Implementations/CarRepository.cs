using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations;

public class CarRepository(VivuCarDbContext dbContext) : ICarRepository
{
    public async Task<(IReadOnlyList<Car> Items, int TotalItems)> GetAdminCarsAsync(
        AdminCarSearchCriteria query,
        CancellationToken cancellationToken = default
    )
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var cars = dbContext.Cars
            .AsNoTracking()
            .Include(car => car.Owner)
            .Include(car => car.CarBrand)
            .Include(car => car.CarModel)
            .Include(car => car.CarType)
            .Include(car => car.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            cars = cars.Where(car =>
                car.Name.Contains(keyword)
                || car.LicensePlate.Contains(keyword)
                || car.CarBrand.Name.Contains(keyword)
                || car.CarModel.Name.Contains(keyword)
                || (car.CarType != null && car.CarType.Name.Contains(keyword))
                || car.Owner.FullName.Contains(keyword));
        }

        if (TryMapStatus(query.Status, out var status))
        {
            cars = cars.Where(car => car.Status == status);
        }

        if (query.TypeId.HasValue)
        {
            cars = cars.Where(car => car.CarTypeId == query.TypeId.Value);
        }

        if (TryMapFuelType(query.FuelType, out var fuelType))
        {
            cars = cars.Where(car => car.FuelType == fuelType);
        }

        if (TryMapTransmission(query.Transmission, out var transmissionType))
        {
            cars = cars.Where(car => car.TransmissionType == transmissionType);
        }

        var totalItems = await cars.CountAsync(cancellationToken);
        var items = await cars
            .OrderByDescending(car => car.CreatedAt)
            .ThenByDescending(car => car.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public Task<Car?> FindByIdWithDetailsAsync(
        int carId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Cars
            .Include(car => car.Owner)
            .Include(car => car.CarBrand)
            .Include(car => car.CarModel)
            .Include(car => car.CarType)
            .Include(car => car.Images)
            .SingleOrDefaultAsync(car => car.Id == carId, cancellationToken);
    }

    public Task<bool> LicensePlateExistsAsync(
        string licensePlate,
        int? exceptCarId = null,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedPlate = licensePlate.Trim().ToUpperInvariant();
        return dbContext.Cars.AnyAsync(
            car => car.LicensePlate.ToUpper() == normalizedPlate
                && (!exceptCarId.HasValue || car.Id != exceptCarId.Value),
            cancellationToken
        );
    }

    public Task<bool> HasActiveBookingAsync(
        int carId,
        CancellationToken cancellationToken = default
    )
    {
        var activeStatuses = new[]
        {
            BookingStatus.PendingApproval,
            BookingStatus.WaitingDeposit,
            BookingStatus.WaitingPickup,
            BookingStatus.InProgress,
            BookingStatus.ReturnRequested
        };

        return dbContext.Bookings.AnyAsync(
            booking => booking.CarId == carId && activeStatuses.Contains(booking.Status),
            cancellationToken
        );
    }

    public Task<AppUser?> FindOwnerAsync(
        int ownerId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Users.SingleOrDefaultAsync(
            user => user.Id == ownerId && user.Role == UserRole.CarOwner,
            cancellationToken
        );
    }

    public Task<CarBrand?> FindBrandByNameAsync(
        string brandName,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedName = brandName.Trim().ToUpperInvariant();
        return dbContext.CarBrands.SingleOrDefaultAsync(
            brand => brand.Name.ToUpper() == normalizedName,
            cancellationToken
        );
    }

    public Task<CarModel?> FindModelByNameAsync(
        int brandId,
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedName = modelName.Trim().ToUpperInvariant();
        return dbContext.CarModels
            .Include(model => model.CarBrand)
            .SingleOrDefaultAsync(
                model => model.CarBrandId == brandId && model.Name.ToUpper() == normalizedName,
                cancellationToken
            );
    }

    public Task<CarModel?> FindModelByIdAsync(
        int modelId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.CarModels
            .Include(model => model.CarBrand)
            .SingleOrDefaultAsync(model => model.Id == modelId, cancellationToken);
    }

    public Task<CarType?> FindTypeByIdAsync(
        int typeId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.CarTypes.SingleOrDefaultAsync(type => type.Id == typeId, cancellationToken);
    }

    public async Task<IReadOnlyList<CarModel>> GetActiveCarModelsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.CarModels
            .AsNoTracking()
            .Include(model => model.CarBrand)
            .Where(model => model.IsActive && model.CarBrand.IsActive)
            .OrderBy(model => model.CarBrand.Name)
            .ThenBy(model => model.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CarType>> GetActiveCarTypesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.CarTypes
            .AsNoTracking()
            .Where(type => type.IsActive)
            .OrderBy(type => type.Name)
            .ToListAsync(cancellationToken);
    }

    public Task AddCarAsync(Car car, CancellationToken cancellationToken = default)
    {
        return dbContext.Cars.AddAsync(car, cancellationToken).AsTask();
    }

    public Task AddBrandAsync(CarBrand brand, CancellationToken cancellationToken = default)
    {
        return dbContext.CarBrands.AddAsync(brand, cancellationToken).AsTask();
    }

    public Task AddModelAsync(CarModel model, CancellationToken cancellationToken = default)
    {
        return dbContext.CarModels.AddAsync(model, cancellationToken).AsTask();
    }

    public Task AddImageAsync(CarImage image, CancellationToken cancellationToken = default)
    {
        return dbContext.CarImages.AddAsync(image, cancellationToken).AsTask();
    }

    public Task AddAuditLogAsync(AdminAuditLog auditLog, CancellationToken cancellationToken = default)
    {
        return dbContext.AdminAuditLogs.AddAsync(auditLog, cancellationToken).AsTask();
    }

    public void RemoveImage(CarImage image)
    {
        dbContext.CarImages.Remove(image);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static bool TryMapStatus(string? value, out CarStatus status)
    {
        status = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        status = value.Trim().ToLowerInvariant() switch
        {
            "available" => CarStatus.Available,
            "rented" => CarStatus.Rented,
            "maintenance" => CarStatus.Maintenance,
            "blocked" => CarStatus.Blocked,
            "unavailable" => CarStatus.Unavailable,
            _ => default
        };

        return status != default;
    }

    private static bool TryMapFuelType(string? value, out FuelType fuelType)
    {
        fuelType = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        return Enum.TryParse(value, ignoreCase: true, out fuelType);
    }

    private static bool TryMapTransmission(string? value, out TransmissionType transmissionType)
    {
        transmissionType = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized == "cvt")
        {
            transmissionType = TransmissionType.Cvt;
            return true;
        }

        return Enum.TryParse(value, ignoreCase: true, out transmissionType);
    }
}
