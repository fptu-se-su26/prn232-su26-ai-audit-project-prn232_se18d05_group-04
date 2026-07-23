namespace Repositories.Models;

public sealed class AdminReportFilter
{
    public string Type { get; init; } = "payments";
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
    public string? PaymentStatus { get; init; }
    public string? BookingStatus { get; init; }
}

public sealed class AdminReportPreviewResponse
{
    public required IReadOnlyList<string> Headers { get; init; }
    public required IReadOnlyList<IReadOnlyList<string>> Rows { get; init; }
    public int TotalRows { get; init; }
}
