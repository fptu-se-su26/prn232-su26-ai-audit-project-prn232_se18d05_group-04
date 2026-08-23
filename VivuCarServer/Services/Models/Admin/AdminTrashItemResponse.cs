using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public sealed record AdminTrashItemResponse(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("info")] string Info,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("deleted_at")] DateTime DeletedAt
);