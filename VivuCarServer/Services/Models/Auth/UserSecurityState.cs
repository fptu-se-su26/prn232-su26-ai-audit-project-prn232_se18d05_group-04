using BusinessObjects.Enums;

namespace Services.Models.Auth;

public sealed record UserSecurityState(
    UserStatus Status,
    int TokenVersion
);
