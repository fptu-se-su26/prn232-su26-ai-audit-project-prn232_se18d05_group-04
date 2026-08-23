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
            .Where(user => user.Status != UserStatus.Deleted)
            .Where(user => !expectedRole.HasValue || user.Role == expectedRole.Value)
            .Select(user => new AdminUserResponse(
                user.Id,
                user.Email,
                user.FullName,
                MapRole(user.Role),
                user.Status == UserStatus.Locked,
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

        if (user is null || user.Status == UserStatus.Deleted)
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

        if (user is null || user.Status == UserStatus.Deleted)
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

    public async Task<bool> SoftDeleteCustomerAsync(
        int userId,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null || user.Role != UserRole.Customer)
        {
            return false;
        }

        if (user.Status == UserStatus.Deleted)
        {
            return true;
        }

        var deletedAt = DateTime.UtcNow;
        await refreshTokenRepository.RevokeAllActiveAsync(
            userId,
            deletedAt,
            ipAddress,
            cancellationToken
        );

        user.Status = UserStatus.Deleted;
        user.TokenVersion++;
        user.UpdatedAt = deletedAt;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(userId);

        return true;
    }

    public async Task<IReadOnlyList<AdminTrashItemResponse>> GetDeletedCustomersAsync(
        CancellationToken cancellationToken = default
    )
    {
        var users = await userRepository.GetByStatusAsync(
            UserStatus.Deleted,
            cancellationToken
        );

        return users
            .Where(user => user.Role == UserRole.Customer)
            .Select(user => new AdminTrashItemResponse(
                "users",
                user.Id,
                user.FullName,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt ?? user.CreatedAt
            ))
            .ToList();
    }

    public async Task<bool> RestoreCustomerAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (
            user is null
            || user.Role != UserRole.Customer
            || user.Status != UserStatus.Deleted
        )
        {
            return false;
        }

        user.Status = UserStatus.Active;
        user.TokenVersion++;
        user.UpdatedAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(userId);

        return true;
    }

    private static string MapRole(UserRole role) => role switch
    {
        UserRole.Customer => "user",
        UserRole.CarOwner => "car_owner",
        UserRole.Admin => "admin",
        _ => "user"
    };
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


