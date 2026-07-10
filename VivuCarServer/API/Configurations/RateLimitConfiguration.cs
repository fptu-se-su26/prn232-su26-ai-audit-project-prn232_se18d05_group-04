using System.Threading.RateLimiting;

namespace API.Configurations;

public static class RateLimitConfiguration
{
    public const string LoginPolicy = "auth-login";

    public static IServiceCollection AddVivuCarRateLimiting(
        this IServiceCollection services
    )
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(
                LoginPolicy,
                context => RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true,
                    }
                )
            );
        });

        return services;
    }
}
