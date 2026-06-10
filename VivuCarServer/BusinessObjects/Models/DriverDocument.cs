using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class DriverDocument
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CitizenIdNumber { get; set; } = string.Empty;
    public string? CitizenIdFrontImageUrl { get; set; }
    public string? CitizenIdBackImageUrl { get; set; }
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public string? DriverLicenseImageUrl { get; set; }
    public DocumentVerificationStatus VerificationStatus { get; set; } = DocumentVerificationStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public AppUser User { get; set; } = null!;
}
