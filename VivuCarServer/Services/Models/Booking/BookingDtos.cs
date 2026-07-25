using System.ComponentModel.DataAnnotations;

namespace Services.Models.Booking;

public class DriverInfoDto
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string CitizenIdNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CitizenIdFrontImageUrl { get; set; }

    [MaxLength(500)]
    public string? CitizenIdBackImageUrl { get; set; }

    [Required]
    [MaxLength(30)]
    public string DriverLicenseNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? DriverLicenseFrontImageUrl { get; set; }

    [MaxLength(500)]
    public string? DriverLicenseBackImageUrl { get; set; }
}

public class PricePreviewRequest
{
    public int CarId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasDelivery { get; set; }
    public decimal DistanceKm { get; set; }
    public string? VoucherCode { get; set; }
}

public class PricePreviewResponse
{
    public int RentalDays { get; set; }
    public int RentalHours { get; set; }
    public int WeekdayCount { get; set; }
    public int WeekendCount { get; set; }
    public decimal WeekdayPrice { get; set; }
    public decimal WeekendPrice { get; set; }
    public decimal HourlyPrice { get; set; }
    public decimal WeekdayCost { get; set; }
    public decimal WeekendCost { get; set; }
    public decimal HourlyCost { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount { get; set; }
}

public class CreateBookingRequest
{
    public int CarId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    [Required]
    [MaxLength(300)]
    public string PickupLocation { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string ReturnLocation { get; set; } = string.Empty;

    public bool HasInsurance { get; set; }
    public bool HasDelivery { get; set; }
    public decimal DistanceKm { get; set; }
    public string? VoucherCode { get; set; }

    [Required]
    public DriverInfoDto DriverInfo { get; set; } = null!;
}

public class BookingDetailResponse
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public string CarName { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string? CarImageUrl { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string ReturnLocation { get; set; } = string.Empty;
    public int RentalDays { get; set; }
    public int RentalHours { get; set; }
    public decimal BasePrice { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DriverInfoDto? DriverInfo { get; set; }
    public string? VoucherCode { get; set; }
    public string? ContractNumber { get; set; }
    public string? ContractPdfUrl { get; set; }
}

public class CancelBookingRequest
{
    [Required]
    public string Reason { get; set; } = string.Empty;
    public string? Note { get; set; }

    [Required]
    public bool Confirmed { get; set; }
}

public class CancelBookingResponse
{
    public int BookingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool CarAvailabilityReleased { get; set; }
    public decimal RefundAmount { get; set; }
}

public class BookingListFilter
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
