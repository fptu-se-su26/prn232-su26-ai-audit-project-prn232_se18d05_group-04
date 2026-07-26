using System.ComponentModel.DataAnnotations;

namespace Services.Models.User;

public class SubmitDocumentRequest
{
    [Required]
    [MaxLength(50)]
    public string CitizenIdNumber { get; set; } = string.Empty;
    
    public string? CitizenIdFrontImageUrl { get; set; }
    
    public string? CitizenIdBackImageUrl { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DriverLicenseNumber { get; set; } = string.Empty;
    
    [Required]
    public string DriverLicenseFrontImageUrl { get; set; } = string.Empty;
    
    [Required]
    public string DriverLicenseBackImageUrl { get; set; } = string.Empty;
}
