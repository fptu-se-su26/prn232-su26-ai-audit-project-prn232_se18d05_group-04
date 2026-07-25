using System.ComponentModel.DataAnnotations;

namespace Services.Models.Auth;

public record VerifyOtpRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6), MaxLength(6)] string Code
);
