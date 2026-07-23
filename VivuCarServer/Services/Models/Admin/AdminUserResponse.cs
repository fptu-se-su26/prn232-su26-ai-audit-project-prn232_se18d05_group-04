using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public sealed record AdminUserResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("is_blocked")] bool IsBlocked,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt
);
