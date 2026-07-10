namespace Services.Models.Auth;

public sealed record AccessTokenResult(
    string AccessToken,
    DateTime ExpiresAt,
    string Jti
);
