using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class PaymentTransaction
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PaymentUrl { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? RawRequest { get; set; }
    public string? RawResponse { get; set; }
    public DateTime CreatedAt { get; set; }

    public Booking Booking { get; set; } = null!;
}
