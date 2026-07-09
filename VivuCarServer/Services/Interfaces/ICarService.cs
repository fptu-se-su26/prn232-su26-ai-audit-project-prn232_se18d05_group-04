using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Services.Models.Car;

namespace Services.Interfaces;

public interface ICarService
{
    Task<CarDetailDto?> GetCarDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<SearchCarsPagedResult> SearchCarsAsync(CarSearchQuery query, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<CarSearchItemDto>> GetFeaturedCarsAsync(string? sortBy, int limit, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default);
}
