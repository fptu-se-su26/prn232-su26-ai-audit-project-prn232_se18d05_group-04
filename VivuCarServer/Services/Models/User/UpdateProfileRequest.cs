using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.User;

public class UpdateProfileRequest
{
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }
}
