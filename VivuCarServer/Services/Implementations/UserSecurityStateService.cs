using Microsoft.Extensions.Caching.Memory;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Auth;

namespace Services.Implementations;

public class UserSecurityStateService(
    IUserRepository userRepository,
    IMemoryCache memoryCache
) : IUserSecurityStateService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public async Task<UserSecurityState?> GetAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = GetCacheKey(userId);

        if (memoryCache.TryGetValue(cacheKey, out UserSecurityState? cachedState))
        {
            return cachedState;
        }

        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var state = new UserSecurityState(user.Status, user.TokenVersion);
        memoryCache.Set(cacheKey, state, CacheDuration);

        return state;
    }

    public void Invalidate(int userId)
    {
        memoryCache.Remove(GetCacheKey(userId));
    }

    private static string GetCacheKey(int userId)
    {
        return $"user-security-state:{userId}";
    }
}
