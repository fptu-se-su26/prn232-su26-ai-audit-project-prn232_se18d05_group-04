using BusinessObjects.Models;

namespace Repositories.Models;

public sealed class AdminRecentBookingRecord
{
    public int Id { get; init; }
    public string BookingCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CarName { get; init; } = string.Empty;
    public DateTime PickupDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string BookingStatus { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = "pending";
}
