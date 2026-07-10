using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminUserService
{
    Task<IReadOnlyList<AdminUserResponse>> GetUsersAsync(
        string? role = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> LockAsync(
        int userId,
        int actingAdminId,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    Task<bool> UnlockAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
