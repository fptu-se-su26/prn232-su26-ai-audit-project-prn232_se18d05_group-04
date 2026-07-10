using BusinessObjects.Enums;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Admin;

namespace Services.Implementations;

public class AdminUserService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserSecurityStateService userSecurityStateService
) : IAdminUserService
{
    public async Task<IReadOnlyList<AdminUserResponse>> GetUsersAsync(
        string? role = null,
        CancellationToken cancellationToken = default
    )
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        var expectedRole = ParseRole(role);

        return users
            .Where(user => !expectedRole.HasValue || user.Role == expectedRole.Value)
            .Select(user => new AdminUserResponse(
                user.Id,
                user.Email,
                user.FullName,
                user.Role.ToString(),
                user.Status.ToString(),
                user.CreatedAt
            ))
            .ToList();
    }

    public async Task<bool> LockAsync(
        int userId,
        int actingAdminId,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        if (userId == actingAdminId)
        {
            return false;
        }

        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        var revokedAt = DateTime.UtcNow;
        await refreshTokenRepository.RevokeAllActiveAsync(
            userId,
            revokedAt,
            ipAddress,
            cancellationToken
        );

        if (user.Status == UserStatus.Locked)
        {
            userSecurityStateService.Invalidate(userId);
            return true;
        }

        user.Status = UserStatus.Locked;
        user.TokenVersion++;
        user.UpdatedAt = revokedAt;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(userId);

        return true;
    }

    public async Task<bool> UnlockAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        if (user.Status == UserStatus.Active)
        {
            return true;
        }

        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(userId);

        return true;
    }

    private static UserRole? ParseRole(string? role)
    {
        return role?.Trim().ToLowerInvariant() switch
        {
            null or "" => null,
            "user" or "customer" => UserRole.Customer,
            "car_owner" or "carowner" => UserRole.CarOwner,
            "admin" => UserRole.Admin,
            _ => null
        };
    }
}
