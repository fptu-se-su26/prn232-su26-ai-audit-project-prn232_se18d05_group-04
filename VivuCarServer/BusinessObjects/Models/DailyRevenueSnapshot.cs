namespace BusinessObjects.Models;

public class DailyRevenueSnapshot
{
    public int Id { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal DepositCollected { get; set; }
    public DateTime GeneratedAt { get; set; }
}
