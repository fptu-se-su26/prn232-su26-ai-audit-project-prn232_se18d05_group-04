namespace BusinessObjects.Models;

public class BookingVoucher
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int VoucherId { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime AppliedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public Voucher Voucher { get; set; } = null!;
}
