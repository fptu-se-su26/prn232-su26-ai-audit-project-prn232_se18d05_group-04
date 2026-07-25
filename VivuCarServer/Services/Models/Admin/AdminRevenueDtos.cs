namespace Services.Models.Admin;

public sealed class AdminRevenueReportResponse
{
    public required DateOnly From { get; init; }
    public required DateOnly To { get; init; }
    public required AdminRevenueSummary Summary { get; init; }
    public required IReadOnlyList<AdminRevenueDailyItem> Daily { get; init; }
    public required IReadOnlyList<AdminRevenueHourlyItem> Hourly { get; init; }
    public required IReadOnlyList<AdminRevenueRecentBooking> RecentBookings { get; init; }
    public required AdminRevenuePagination Pagination { get; init; }
}

public sealed class AdminRevenueHourlyItem
{
    public int Hour { get; init; }
    public decimal GrossRevenue { get; init; }
    public int PaidTransactions { get; init; }
}
public sealed class AdminRevenuePagination
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}

public sealed class AdminRevenueSummary
{
    public decimal GrossRevenue { get; init; }
    public decimal PendingPaymentAmount { get; init; }
    public decimal CancelledAmount { get; init; }
    public decimal NetRevenue { get; init; }
    public decimal DepositCollected { get; init; }
    public int TotalBookings { get; init; }
    public int PaidBookings { get; init; }
    public int PendingPaymentBookings { get; init; }
    public int CancelledWithoutPaymentBookings { get; init; }
    public int CompletedBookings { get; init; }
    public int CancelledBookings { get; init; }
    public decimal AverageOrderValue { get; init; }
    public decimal CancelRate { get; init; }
}

public sealed class AdminRevenueDailyItem
{
    public DateOnly Date { get; init; }
    public decimal GrossRevenue { get; init; }
    public decimal PendingPaymentAmount { get; init; }
    public decimal CancelledAmount { get; init; }
    public decimal NetRevenue { get; init; }
    public decimal DepositCollected { get; init; }
    public int TotalBookings { get; init; }
    public int PaidBookings { get; init; }
    public int PendingPaymentBookings { get; init; }
    public int CompletedBookings { get; init; }
    public int CancelledBookings { get; init; }
}

public sealed class AdminRevenueRecentBooking
{
    public int Id { get; init; }
    public required string BookingCode { get; init; }
    public required string CustomerName { get; init; }
    public required string CarName { get; init; }
    public DateTime PickupDate { get; init; }
    public decimal TotalAmount { get; init; }
    public required string BookingStatus { get; init; }
    public required string PaymentStatus { get; init; }
}
