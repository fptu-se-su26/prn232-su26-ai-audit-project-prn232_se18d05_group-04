using System.Data;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class RefreshTokenRepository(VivuCarDbContext dbContext)
    : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken
            );
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<bool> TryRotateAsync(
        RefreshToken currentToken,
        RefreshToken replacementToken,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken
        );
        var updatedRows = await dbContext.RefreshTokens
            .Where(token => token.Id == currentToken.Id && token.RevokedAt == null)
            .ExecuteUpdateAsync(
                updates => updates
                    .SetProperty(token => token.RevokedAt, revokedAt)
                    .SetProperty(token => token.RevokedByIp, revokedByIp)
                    .SetProperty(
                        token => token.ReplacedByTokenHash,
                        replacementToken.TokenHash
                    ),
                cancellationToken
            );

        if (updatedRows != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await dbContext.RefreshTokens.AddAsync(replacementToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task RevokeAllActiveAsync(
        int userId,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null)
            .ExecuteUpdateAsync(
                updates => updates
                    .SetProperty(token => token.RevokedAt, revokedAt)
                    .SetProperty(token => token.RevokedByIp, revokedByIp),
                cancellationToken
            );
    }

    public async Task RevokeByHashAsync(
        string tokenHash,
        DateTime revokedAt,
        string? revokedByIp,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.RefreshTokens
            .Where(token => token.TokenHash == tokenHash && token.RevokedAt == null)
            .ExecuteUpdateAsync(
                updates => updates
                    .SetProperty(token => token.RevokedAt, revokedAt)
                    .SetProperty(token => token.RevokedByIp, revokedByIp),
                cancellationToken
            );
    }

    public Task<int> DeleteObsoleteAsync(
        DateTime expiredBefore,
        DateTime revokedBefore,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.RefreshTokens
            .Where(token =>
                token.ExpiresAt <= expiredBefore
                || (
                    token.RevokedAt != null
                    && token.RevokedAt <= revokedBefore
                )
            )
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
