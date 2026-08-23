namespace Services.Models.Auth;

public sealed record AuthenticatedUserResponse(
    int Id,
    string Email,
    string FullName,
    string Role
);
