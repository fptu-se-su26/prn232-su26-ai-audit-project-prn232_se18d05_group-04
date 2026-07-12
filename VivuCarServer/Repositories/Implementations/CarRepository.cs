using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        if (query.OwnerId.HasValue)
        {
            cars = cars.Where(car => car.OwnerId == query.OwnerId.Value);
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

    public async Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Cars
            .Include(c => c.CarBrand)
            .Include(c => c.CarModel)
            .Include(c => c.Images)
            .Include(c => c.Owner)
            .Include(c => c.Reviews)
                .ThenInclude(r => r.Customer)
            .Include(c => c.AvailabilityBlocks)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Car> Items, int TotalCount)> SearchCarsAsync(
        string? searchTerm,
        int? brandId,
        decimal? minPrice,
        decimal? maxPrice,
        string? location,
        int? transmissionType,
        int? fuelType,
        int? seatCount,
        double? minRating,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        // Only show available cars that are not locked, pending or unavailable
        var query = dbContext.Cars
            .Include(c => c.CarBrand)
            .Include(c => c.CarModel)
            .Include(c => c.Images)
            .Include(c => c.Reviews)
            .Include(c => c.Owner)
            .Where(c => c.Status == CarStatus.Available);

        // Filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search) || 
                                     c.CarBrand.Name.ToLower().Contains(search) || 
                                     c.CarModel.Name.ToLower().Contains(search));
        }

        if (brandId.HasValue)
        {
            query = query.Where(c => c.CarBrandId == brandId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.DailyPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.DailyPrice <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var loc = location.Trim().ToLower();
            query = query.Where(c => c.Location.ToLower().Contains(loc));
        }

        if (transmissionType.HasValue)
        {
            var transmission = (TransmissionType)transmissionType.Value;
            query = query.Where(c => c.TransmissionType == transmission);
        }

        if (fuelType.HasValue)
        {
            var fuel = (FuelType)fuelType.Value;
            query = query.Where(c => c.FuelType == fuel);
        }

        if (seatCount.HasValue)
        {
            query = query.Where(c => c.SeatCount == seatCount.Value);
        }

        if (minRating.HasValue && minRating.Value > 0)
        {
            query = query.Where(c => c.Reviews.Any() && c.Reviews.Average(r => r.Rating) >= minRating.Value);
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            var start = startDate.Value;
            var end = endDate.Value;
            
            // Exclude cars that have availability block or active booking overlaps
            query = query.Where(c => 
                !c.AvailabilityBlocks.Any(ab => ab.StartDateTime < end && ab.EndDateTime > start) &&
                !c.Bookings.Any(b => b.Status != BookingStatus.Cancelled 
                                    && b.Status != BookingStatus.Rejected 
                                    && b.Status != BookingStatus.Expired 
                                    && b.StartDateTime < end 
                                    && b.EndDateTime > start)
            );
        }

        // Sorting
        // Options: price_asc, price_desc, rating_desc, bookings_desc, newest
        if (string.Equals(sortBy, "price_asc", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderBy(c => c.DailyPrice);
        }
        else if (string.Equals(sortBy, "price_desc", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(c => c.DailyPrice);
        }
        else if (string.Equals(sortBy, "rating_desc", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0);
        }
        else if (string.Equals(sortBy, "bookings_desc", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(c => c.Bookings.Count);
        }
        else if (string.Equals(sortBy, "newest", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(c => c.CreatedAt);
        }
        else
        {
            // Default sort
            query = query.OrderByDescending(c => c.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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

    public async Task<IReadOnlyList<Car>> GetFeaturedCarsAsync(string? sortBy, int limit, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Cars
            .Include(c => c.CarBrand)
            .Include(c => c.CarModel)
            .Include(c => c.Images)
            .Include(c => c.Reviews)
            .Include(c => c.Owner)
            .Where(c => c.Status == CarStatus.Available);

        if (string.Equals(sortBy, "bookings", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(c => c.Bookings.Count);
        }
        else
        {
            // Default sort by rating
            query = query.OrderByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0);
        }

        return await query.Take(limit).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<string>();
        }

        var search = query.Trim().ToLower();

        // Suggest combinations of Brand + Model or Car names
        var brandSuggestions = await dbContext.CarBrands
            .Where(b => b.IsActive && b.Name.ToLower().Contains(search))
            .Select(b => b.Name)
            .Take(5)
            .ToListAsync(cancellationToken);

        var modelSuggestions = await dbContext.CarModels
            .Where(m => m.IsActive && (m.Name.ToLower().Contains(search) || m.CarBrand.Name.ToLower().Contains(search)))
            .Select(m => $"{m.CarBrand.Name} {m.Name}")
            .Take(5)
            .ToListAsync(cancellationToken);

        var carSuggestions = await dbContext.Cars
            .Where(c => c.Status == CarStatus.Available && c.Name.ToLower().Contains(search))
            .Select(c => c.Name)
            .Take(5)
            .ToListAsync(cancellationToken);

        return brandSuggestions.Concat(modelSuggestions).Concat(carSuggestions)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();
    }
}
