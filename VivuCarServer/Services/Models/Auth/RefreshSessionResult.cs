namespace Services.Models.Auth;

public enum RefreshSessionStatus
{
    Success = 1,
    Invalid = 2,
    ReuseDetected = 3,
}

public sealed record RefreshSessionResult(
    RefreshSessionStatus Status,
    AuthSessionResult? Session = null
);
