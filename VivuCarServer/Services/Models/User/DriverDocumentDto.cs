using System;
using BusinessObjects.Enums;

namespace Services.Models.User;

public class DriverDocumentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CitizenIdNumber { get; set; } = string.Empty;
    public string? CitizenIdFrontImageUrl { get; set; }
    public string? CitizenIdBackImageUrl { get; set; }
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public string? DriverLicenseFrontImageUrl { get; set; }
    public string? DriverLicenseBackImageUrl { get; set; }
    public DocumentVerificationStatus VerificationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
