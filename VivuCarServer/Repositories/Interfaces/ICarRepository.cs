using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<(IReadOnlyList<Car> Items, int TotalCount)> SearchCarsAsync(
        string? searchTerm,
        int? brandId,
        decimal? minPrice,
        decimal? maxPrice,
        string? location,
        int? transmissionType, // 1 = Manual, 2 = Automatic
        int? fuelType,         // enum FuelType
        int? seatCount,
        double? minRating,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,        // price_asc, price_desc, rating_desc, bookings_desc, newest
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    );
    
    Task<IReadOnlyList<Car>> GetFeaturedCarsAsync(string? sortBy, int limit, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default);
}
