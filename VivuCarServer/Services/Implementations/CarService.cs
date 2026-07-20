using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Car;

namespace Services.Implementations;

public class CarService(ICarRepository carRepository) : ICarService
{
    public async Task<CarDetailDto?> GetCarDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await carRepository.GetByIdAsync(id, cancellationToken);
        if (car == null)
        {
            return null;
        }

        var avgRating = car.Reviews.Any() ? Math.Round(car.Reviews.Average(r => r.Rating), 1) : 0.0;
        
        return new CarDetailDto
        {
            Id = car.Id,
            Name = car.Name,
            LicensePlate = car.LicensePlate,
            Description = car.Description,
            Location = car.Location,
            DailyPrice = car.DailyPrice,
            InsuranceFeePerDay = car.InsuranceFeePerDay,
            DeliveryFee = car.DeliveryFee,
            DepositAmount = car.DepositAmount,
            Status = car.Status.ToString(),
            SeatCount = car.SeatCount,
            TransmissionType = car.TransmissionType.ToString(),
            FuelType = car.FuelType.ToString(),
            
            CarBrandId = car.CarBrandId,
            BrandName = car.CarBrand.Name,
            CarModelId = car.CarModelId,
            ModelName = car.CarModel.Name,
            
            OwnerId = car.OwnerId,
            OwnerFullName = car.Owner.FullName,
            OwnerAvatarUrl = car.Owner.AvatarUrl,
            OwnerPhoneNumber = car.Owner.PhoneNumber,
            
            AverageRating = avgRating,
            TotalBookings = car.Bookings?.Count ?? 0,
            
            Images = car.Images.Select(i => new CarImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsPrimary = i.IsPrimary,
                DisplayOrder = i.DisplayOrder
            }).OrderBy(i => i.DisplayOrder).ToList(),
            
            Reviews = car.Reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FullName,
                CustomerAvatarUrl = r.Customer.AvatarUrl
            }).OrderByDescending(r => r.CreatedAt).ToList(),
            
            AvailabilityBlocks = car.AvailabilityBlocks.Select(ab => new AvailabilityBlockDto
            {
                Id = ab.Id,
                StartDateTime = ab.StartDateTime,
                EndDateTime = ab.EndDateTime,
                Reason = ab.Reason
            }).ToList()
        };
    }

    public async Task<SearchCarsPagedResult> SearchCarsAsync(CarSearchQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await carRepository.SearchCarsAsync(
            query.SearchTerm,
            query.Brands,
            query.BrandId,
            query.MinPrice,
            query.MaxPrice,
            query.Location,
            query.TransmissionType,
            query.FuelType,
            query.SeatCount,
            query.MinRating,
            query.StartDate,
            query.EndDate,
            query.SortBy,
            query.Page,
            query.PageSize,
            cancellationToken
        );

        var mappedItems = items.Select(MapToSearchItemDto).ToList();

        return new SearchCarsPagedResult
        {
            Items = mappedItems,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<IReadOnlyList<CarSearchItemDto>> GetFeaturedCarsAsync(string? sortBy, int limit, CancellationToken cancellationToken = default)
    {
        var cars = await carRepository.GetFeaturedCarsAsync(sortBy, limit, cancellationToken);
        return cars.Select(MapToSearchItemDto).ToList();
    }

    public async Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default)
    {
        return await carRepository.GetSearchSuggestionsAsync(query, cancellationToken);
    }

    private static CarSearchItemDto MapToSearchItemDto(Car c)
    {
        var primaryImage = c.Images.FirstOrDefault(i => i.IsPrimary) ?? 
                           c.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();
                           
        var avgRating = c.Reviews.Any() ? Math.Round(c.Reviews.Average(r => r.Rating), 1) : 0.0;

        return new CarSearchItemDto
        {
            Id = c.Id,
            Name = c.Name,
            BrandName = c.CarBrand?.Name ?? string.Empty,
            ModelName = c.CarModel?.Name ?? string.Empty,
            DailyPrice = c.DailyPrice,
            Location = c.Location,
            PrimaryImageUrl = primaryImage?.ImageUrl,
            SeatCount = c.SeatCount,
            Transmission = c.TransmissionType.ToString(),
            Fuel = c.FuelType.ToString(),
            AverageRating = avgRating,
            TotalBookings = c.Bookings?.Count ?? 0,
            OwnerName = c.Owner?.FullName ?? string.Empty
        };
    }
}
