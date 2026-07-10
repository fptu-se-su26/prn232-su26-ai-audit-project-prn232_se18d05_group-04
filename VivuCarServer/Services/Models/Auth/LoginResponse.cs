namespace Services.Models.Auth;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    AuthenticatedUserResponse User
);
