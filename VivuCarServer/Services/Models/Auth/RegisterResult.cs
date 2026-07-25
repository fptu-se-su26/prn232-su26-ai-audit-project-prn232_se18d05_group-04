namespace Services.Models.Auth;

public record RegisterResult(
    bool Success,
    string Message,
    int? UserId = null
);
