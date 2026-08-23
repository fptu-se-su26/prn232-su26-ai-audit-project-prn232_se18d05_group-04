using Services.Models.Auth;

namespace Services.Interfaces;

public interface IUserSecurityStateService
{
    Task<UserSecurityState?> GetAsync(
        int userId,
        CancellationToken cancellationToken = default
    );

    void Invalidate(int userId);
}
