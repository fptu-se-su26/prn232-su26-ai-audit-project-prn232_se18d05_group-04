using BusinessObjects.Models;
using Repositories.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositories.Interfaces;

public interface ICarRepository
{
    Task<(IReadOnlyList<Car> Items, int TotalItems)> GetAdminCarsAsync(
        AdminCarSearchCriteria query,
        CancellationToken cancellationToken = default
    );

    Task<Car?> FindByIdWithDetailsAsync(
        int carId,
        CancellationToken cancellationToken = default
    );

    Task<bool> LicensePlateExistsAsync(
        string licensePlate,
        int? exceptCarId = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasActiveBookingAsync(
        int carId,
        CancellationToken cancellationToken = default
    );

    Task<AppUser?> FindOwnerAsync(
        int ownerId,
        CancellationToken cancellationToken = default
    );

    Task<CarBrand?> FindBrandByNameAsync(
        string brandName,
        CancellationToken cancellationToken = default
    );

    Task<CarModel?> FindModelByNameAsync(
        int brandId,
        string modelName,
        CancellationToken cancellationToken = default
    );

    Task<CarModel?> FindModelByIdAsync(
        int modelId,
        CancellationToken cancellationToken = default
    );

    Task<CarType?> FindTypeByIdAsync(
        int typeId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CarModel>> GetActiveCarModelsAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CarType>> GetActiveCarTypesAsync(
        CancellationToken cancellationToken = default
    );

    Task AddCarAsync(Car car, CancellationToken cancellationToken = default);
    Task AddBrandAsync(CarBrand brand, CancellationToken cancellationToken = default);
    Task AddModelAsync(CarModel model, CancellationToken cancellationToken = default);
    Task AddImageAsync(CarImage image, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(AdminAuditLog auditLog, CancellationToken cancellationToken = default);
    void RemoveImage(CarImage image);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<(IReadOnlyList<Car> Items, int TotalCount)> SearchCarsAsync(
        string? searchTerm,
        string? brands,
        int? brandId,
        decimal? minPrice,
        decimal? maxPrice,
        string? location,
        int? transmissionType,             // 1 = Manual, 2 = Automatic, 3 = CVT
        IReadOnlyList<int>? fuelTypes,     // list of FuelType enum values
        int? seatCount,
        double? minRating,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,                    // price_asc, price_desc, rating_desc, bookings_desc, newest
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    );
    
    Task<IReadOnlyList<Car>> GetFeaturedCarsAsync(string? sortBy, int limit, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<string> Brands, IReadOnlyList<string> Districts, IReadOnlyList<int> SeatCounts, IReadOnlyList<string> Transmissions, IReadOnlyList<string> FuelTypes, decimal MinPrice, decimal MaxPrice)> GetFilterOptionsAsync(CancellationToken cancellationToken = default);
}
