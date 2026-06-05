using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class Booking
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string ReturnLocation { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.PendingApproval;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public AppUser Customer { get; set; } = null!;
    public Car Car { get; set; } = null!;
    public BookingDriverInfo? DriverInfo { get; set; }
    public BookingVoucher? BookingVoucher { get; set; }
    public RentalContract? RentalContract { get; set; }
    public Review? Review { get; set; }
    public ICollection<BookingStatusHistory> StatusHistories { get; set; } = [];
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = [];
    public ICollection<NotificationLog> NotificationLogs { get; set; } = [];
    public ICollection<CarAvailabilityBlock> AvailabilityBlocks { get; set; } = [];
    public ICollection<IncidentReport> IncidentReports { get; set; } = [];
}
