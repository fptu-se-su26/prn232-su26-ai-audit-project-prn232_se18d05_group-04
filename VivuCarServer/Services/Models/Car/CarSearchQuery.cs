using System;

namespace Services.Models.Car;

public class CarSearchQuery
{
    public string? SearchTerm { get; set; }
    public string? Brands { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Location { get; set; }
    public int? TransmissionType { get; set; } // 1 = Manual, 2 = Automatic
    public int? FuelType { get; set; }         // FuelType enum values
    public int? SeatCount { get; set; }
    public double? MinRating { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SortBy { get; set; }        // price_asc, price_desc, rating_desc, bookings_desc, newest
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
