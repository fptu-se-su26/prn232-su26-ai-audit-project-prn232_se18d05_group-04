using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class Car
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int CarBrandId { get; set; }
    public int CarModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public decimal InsuranceFeePerDay { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DepositAmount { get; set; }
    public CarStatus Status { get; set; } = CarStatus.Pending;
    public int SeatCount { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public AppUser Owner { get; set; } = null!;
    public CarBrand CarBrand { get; set; } = null!;
    public CarModel CarModel { get; set; } = null!;
    public ICollection<CarImage> Images { get; set; } = [];
    public ICollection<CarAvailabilityBlock> AvailabilityBlocks { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
