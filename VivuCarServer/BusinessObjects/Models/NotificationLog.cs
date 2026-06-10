using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class NotificationLog
{
    public int Id { get; set; }
    public int? BookingId { get; set; }
    public int RecipientUserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime CreatedAt { get; set; }

    public Booking? Booking { get; set; }
    public AppUser RecipientUser { get; set; } = null!;
}
