using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> FindByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default
    );

    Task<AppUser?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AppUser>> GetAllAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AppUser>> GetByStatusAsync(
        UserStatus status,
        CancellationToken cancellationToken = default
    );

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
