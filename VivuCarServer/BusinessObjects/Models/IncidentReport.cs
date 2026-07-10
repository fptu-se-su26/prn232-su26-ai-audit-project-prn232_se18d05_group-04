using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class IncidentReport
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ReporterId { get; set; }
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public AppUser Reporter { get; set; } = null!;
}
