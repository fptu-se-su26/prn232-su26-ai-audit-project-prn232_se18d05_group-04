using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public class AdminVoucherListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? DiscountType { get; set; }
    public string? Status { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}

public class AdminVoucherUpsertRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("code")] public string Code { get; set; } = string.Empty;
    [JsonPropertyName("discount_type")] public string DiscountType { get; set; } = string.Empty;
    [JsonPropertyName("discount_value")] public decimal DiscountValue { get; set; }
    [JsonPropertyName("min_order_amount")] public decimal MinOrderAmount { get; set; }
    [JsonPropertyName("max_discount")] public decimal MaxDiscount { get; set; }
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
    [JsonPropertyName("expires_at")] public DateTime? ExpiresAt { get; set; }
}

public record AdminVoucherResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("discount_type")] string DiscountType,
    [property: JsonPropertyName("discount_value")] decimal DiscountValue,
    [property: JsonPropertyName("min_order_amount")] decimal MinOrderAmount,
    [property: JsonPropertyName("max_discount")] decimal MaxDiscount,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("usage_count")] int UsageCount,
    [property: JsonPropertyName("remaining_quantity")] int RemainingQuantity,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("expires_at")] DateTime? ExpiresAt,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt
);

public record AdminVoucherDailyUsage(
    [property: JsonPropertyName("date")] DateTime Date,
    [property: JsonPropertyName("usage_count")] int UsageCount,
    [property: JsonPropertyName("discount_total")] decimal DiscountTotal
);

public record AdminVoucherRecentUsage(
    [property: JsonPropertyName("booking_id")] int BookingId,
    [property: JsonPropertyName("booking_code")] string BookingCode,
    [property: JsonPropertyName("customer_name")] string CustomerName,
    [property: JsonPropertyName("order_amount")] decimal OrderAmount,
    [property: JsonPropertyName("discount_amount")] decimal DiscountAmount,
    [property: JsonPropertyName("applied_at")] DateTime AppliedAt
);

public record AdminVoucherPerformanceResponse(
    [property: JsonPropertyName("voucher")] AdminVoucherResponse Voucher,
    [property: JsonPropertyName("usage_rate")] decimal UsageRate,
    [property: JsonPropertyName("gross_revenue")] decimal GrossRevenue,
    [property: JsonPropertyName("discount_total")] decimal DiscountTotal,
    [property: JsonPropertyName("daily_usage")] IReadOnlyList<AdminVoucherDailyUsage> DailyUsage,
    [property: JsonPropertyName("recent_usages")] IReadOnlyList<AdminVoucherRecentUsage> RecentUsages
);
