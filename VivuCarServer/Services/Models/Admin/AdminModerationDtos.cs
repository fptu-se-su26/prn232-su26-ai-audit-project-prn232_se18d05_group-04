using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public record AdminModerationContentResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("reporterName")] string ReporterName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
);

public record AdminLicenseModerationResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("driverLicenseNumber")] string DriverLicenseNumber,
    [property: JsonPropertyName("frontImageUrl")] string? FrontImageUrl,
    [property: JsonPropertyName("backImageUrl")] string? BackImageUrl,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTime? UpdatedAt
);
