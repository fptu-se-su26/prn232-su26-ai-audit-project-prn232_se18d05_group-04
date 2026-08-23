namespace BusinessObjects.Models;

public class AdminAuditLog
{
    public long Id { get; set; }
    public int AdminUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public AppUser AdminUser { get; set; } = null!;
}
