namespace Repositories.Models;

public sealed class AdminRevenueStatusRecord
{
    public DateOnly Date { get; init; }
    public decimal PendingPaymentAmount { get; init; }
    public decimal CancelledAmount { get; init; }
    public int PaidBookings { get; init; }
    public int PendingPaymentBookings { get; init; }
    public int CancelledWithoutPaymentBookings { get; init; }
}

public sealed class AdminHourlyRevenueRecord
{
    public int Hour { get; init; }
    public decimal GrossRevenue { get; init; }
    public int PaidTransactions { get; init; }
}