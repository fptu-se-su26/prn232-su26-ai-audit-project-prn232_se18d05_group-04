using System;
using System.Collections.Generic;

namespace Services.Models.Car;

public class CarDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public decimal InsuranceFeePerDay { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DepositAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SeatCount { get; set; }
    public string TransmissionType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    
    // Brand and Model Info
    public int CarBrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public int CarModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    
    // Owner Info
    public int OwnerId { get; set; }
    public string OwnerFullName { get; set; } = string.Empty;
    public string? OwnerAvatarUrl { get; set; }
    public string OwnerPhoneNumber { get; set; } = string.Empty;
    
    public double AverageRating { get; set; }
    public int TotalBookings { get; set; }
    
    public List<CarImageDto> Images { get; set; } = [];
    public List<ReviewDto> Reviews { get; set; } = [];
    public List<AvailabilityBlockDto> AvailabilityBlocks { get; set; } = [];
}

public class CarImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Customer who left review
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAvatarUrl { get; set; }
}

public class AvailabilityBlockDto
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Reason { get; set; } = string.Empty;
}
