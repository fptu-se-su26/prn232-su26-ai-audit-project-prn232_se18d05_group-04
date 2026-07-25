using System.ComponentModel.DataAnnotations;

namespace Services.Models.Auth;

public record ResendOtpRequest(
    [Required, EmailAddress] string Email
);
