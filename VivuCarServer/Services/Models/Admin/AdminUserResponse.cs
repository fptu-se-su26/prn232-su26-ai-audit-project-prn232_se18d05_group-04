namespace Services.Models.Admin;

public sealed record AdminUserResponse(
    int Id,
    string Email,
    string FullName,
    string Role,
    string Status,
    DateTime CreatedAt
);
