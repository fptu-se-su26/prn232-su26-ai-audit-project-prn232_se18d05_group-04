using API.HostedServices;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddVivuCarAuthentication(
        this IServiceCollection services
    )
    {
        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        // UserSeedHostedService removed — replaced by AppDbSeederHostedService (registered in ServiceConfiguration)
        services.AddHostedService<RefreshTokenCleanupHostedService>();

        return services;
    }
}
