using Services.Models.Auth;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<AuthSessionResult?> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task<RefreshSessionResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task LogoutAsync(
        string? refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task<bool> LogoutAllAsync(
        int userId,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default
    );

    Task<VerifyOtpResult> VerifyOtpAsync(
        VerifyOtpRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task<RegisterResult> ResendOtpAsync(
        ResendOtpRequest request,
        CancellationToken cancellationToken = default
    );
}
