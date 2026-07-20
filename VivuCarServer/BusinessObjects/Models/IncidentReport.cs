using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class IncidentReport
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ReporterId { get; set; }
    
    /// <summary>Tiêu đề ngắn gọn của sự cố.</summary>
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Số tiền bồi thường / phạt do sự cố (nếu có). Null = chưa xác định.</summary>
    public decimal? PenaltyAmount { get; set; }

    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public AppUser Reporter { get; set; } = null!;
}
