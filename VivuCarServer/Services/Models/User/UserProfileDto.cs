using System;
using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace Services.Models.User;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Status info
    public DocumentVerificationStatus DriverDocumentStatus { get; set; }
}
