using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public class AdminCarListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public int? TypeId { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
}

public record PagedResult<T>(
    [property: JsonPropertyName("items")] IReadOnlyList<T> Items,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("page_size")] int PageSize,
    [property: JsonPropertyName("total_items")] int TotalItems,
    [property: JsonPropertyName("total_pages")] int TotalPages
);

public record AdminCarImageResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("car_id")] int CarId,
    [property: JsonPropertyName("image_url")] string ImageUrl,
    [property: JsonPropertyName("is_primary")] bool IsPrimary,
    [property: JsonPropertyName("display_order")] int DisplayOrder
);

public record AdminCarResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("owner_id")] int OwnerId,
    [property: JsonPropertyName("owner_name")] string OwnerName,
    [property: JsonPropertyName("brand")] string Brand,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("car_model_id")] int CarModelId,
    [property: JsonPropertyName("type_id")] int? TypeId,
    [property: JsonPropertyName("type_name")] string? TypeName,
    [property: JsonPropertyName("license_plate")] string LicensePlate,
    [property: JsonPropertyName("year")] int? Year,
    [property: JsonPropertyName("color")] string? Color,
    [property: JsonPropertyName("seats")] int Seats,
    [property: JsonPropertyName("kilometers_driven")] int KilometersDriven,
    [property: JsonPropertyName("transmission")] string Transmission,
    [property: JsonPropertyName("fuel_type")] string FuelType,
    [property: JsonPropertyName("price_per_day")] decimal PricePerDay,
    [property: JsonPropertyName("price_per_hours")] decimal PricePerHours,
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("blocked_reason")] string? BlockedReason,
    [property: JsonPropertyName("primary_image_url")] string? PrimaryImageUrl,
    [property: JsonPropertyName("images")] IReadOnlyList<AdminCarImageResponse> Images,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

public class AdminCarUpsertRequest
{
    [JsonPropertyName("owner_id")]
    public int OwnerId { get; set; }

    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("car_model_id")]
    public int? CarModelId { get; set; }

    [JsonPropertyName("type_id")]
    public int? TypeId { get; set; }

    [JsonPropertyName("license_plate")]
    public string LicensePlate { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("seats")]
    public int Seats { get; set; }

    [JsonPropertyName("kilometers_driven")]
    public int? KilometersDriven { get; set; }

    [JsonPropertyName("transmission")]
    public string Transmission { get; set; } = string.Empty;

    [JsonPropertyName("fuel_type")]
    public string FuelType { get; set; } = string.Empty;

    [JsonPropertyName("price_per_day")]
    public decimal PricePerDay { get; set; }

    [JsonPropertyName("price_per_hours")]
    public decimal PricePerHours { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "available";
}

public class AdminCarBlockRequest
{
    [JsonPropertyName("blocked_reason")]
    public string? BlockedReason { get; set; }
}

public class AdminCarUnblockRequest
{
    [JsonPropertyName("target_status")]
    public string TargetStatus { get; set; } = "available";
}

public class AdminCarImageCreateRequest
{
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; set; }
}

public record CarTypeResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);

public record CarModelResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("brand_id")] int BrandId,
    [property: JsonPropertyName("brand_name")] string BrandName
);
