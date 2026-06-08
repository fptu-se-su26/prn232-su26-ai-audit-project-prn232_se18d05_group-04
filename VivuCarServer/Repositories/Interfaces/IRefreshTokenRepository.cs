using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    );

    Task<bool> TryRotateAsync(
        RefreshToken currentToken,
        RefreshToken replacementToken,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    );

    Task RevokeAllActiveAsync(
        int userId,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    );

    Task RevokeByHashAsync(
        string tokenHash,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    );

    Task<int> DeleteObsoleteAsync(
        DateTime expiredBefore,
        DateTime revokedBefore,
        CancellationToken cancellationToken = default
    );

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
