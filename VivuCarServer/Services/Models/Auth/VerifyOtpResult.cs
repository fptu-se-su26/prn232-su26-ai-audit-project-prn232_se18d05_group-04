namespace Services.Models.Auth;

public record VerifyOtpResult(
    bool Success,
    string Message,
    AuthSessionResult? Session = null
);
