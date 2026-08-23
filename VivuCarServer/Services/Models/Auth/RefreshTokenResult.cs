namespace Services.Models.Auth;

public sealed record RefreshTokenResult(
    string Token,
    string TokenHash,
    DateTime ExpiresAt
);
