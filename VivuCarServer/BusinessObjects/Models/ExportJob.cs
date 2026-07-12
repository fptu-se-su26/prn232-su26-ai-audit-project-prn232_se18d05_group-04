namespace BusinessObjects.Models;

public class ExportJob
{
    public int Id { get; set; }
    public int RequestedBy { get; set; }
    public string ExportType { get; set; } = string.Empty;
    public string ParamsJson { get; set; } = "{}";
    public string Status { get; set; } = "pending";
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public AppUser Requester { get; set; } = null!;
}
