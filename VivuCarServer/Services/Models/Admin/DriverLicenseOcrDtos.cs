using System.Text.Json.Serialization;

namespace Services.Models.Admin;

public sealed record DriverLicenseOcrResult(
    string RawText,
    string? FullName,
    string? LicenseNumber,
    string? DateOfBirth,
    string? LicenseClass,
    string? ExpiryDate,
    float Confidence,
    DateTime ProcessedAt);

public sealed record AdminLicenseOcrResponse(
    [property: JsonPropertyName("documentId")] int DocumentId,
    [property: JsonPropertyName("fullName")] string? FullName,
    [property: JsonPropertyName("licenseNumber")] string? LicenseNumber,
    [property: JsonPropertyName("dateOfBirth")] string? DateOfBirth,
    [property: JsonPropertyName("licenseClass")] string? LicenseClass,
    [property: JsonPropertyName("expiryDate")] string? ExpiryDate,
    [property: JsonPropertyName("confidence")] float Confidence,
    [property: JsonPropertyName("processedAt")] DateTime ProcessedAt,
    [property: JsonPropertyName("rawText")] string RawText);
