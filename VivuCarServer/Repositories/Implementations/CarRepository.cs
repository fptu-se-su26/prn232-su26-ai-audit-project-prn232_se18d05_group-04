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

namespace Repositories.Implementations;

public class CarRepository(VivuCarDbContext dbContext) : ICarRepository
{
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
