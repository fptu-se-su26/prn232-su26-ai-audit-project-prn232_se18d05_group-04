using System.ComponentModel.DataAnnotations;

namespace Services.Models.Auth;

public record RegisterRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(128)] string Password,
    [Required, MaxLength(150)] string FullName,
    [Required, MaxLength(20)] string PhoneNumber,
    DateOnly? DateOfBirth = null
);
