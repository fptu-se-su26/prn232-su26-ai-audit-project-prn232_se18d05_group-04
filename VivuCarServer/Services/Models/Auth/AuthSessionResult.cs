namespace Services.Models.Auth;

public sealed record AuthSessionResult(
    LoginResponse Response,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
