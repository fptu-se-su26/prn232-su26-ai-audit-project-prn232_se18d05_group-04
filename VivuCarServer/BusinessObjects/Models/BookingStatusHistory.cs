using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class BookingStatusHistory
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public BookingStatus OldStatus { get; set; }
    public BookingStatus NewStatus { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public AppUser? ChangedByUser { get; set; }
}
