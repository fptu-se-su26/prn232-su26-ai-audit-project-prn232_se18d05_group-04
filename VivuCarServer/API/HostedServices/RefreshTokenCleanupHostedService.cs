using System.Globalization;
using Repositories.Interfaces;

namespace API.HostedServices;

public class RefreshTokenCleanupHostedService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<RefreshTokenCleanupHostedService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cleanupInterval = TimeSpan.FromHours(
            GetPositiveNumber("AUTH_TOKEN_CLEANUP_HOURS", 24)
        );
        var revokedRetention = TimeSpan.FromDays(
            GetPositiveNumber("AUTH_REVOKED_TOKEN_RETENTION_DAYS", 7)
        );

        await CleanupAsync(revokedRetention, stoppingToken);

        using var timer = new PeriodicTimer(cleanupInterval);

        while (
            await timer.WaitForNextTickAsync(stoppingToken)
        )
        {
            await CleanupAsync(revokedRetention, stoppingToken);
        }
    }

    private async Task CleanupAsync(
        TimeSpan revokedRetention,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider
                .GetRequiredService<IRefreshTokenRepository>();
            var now = DateTime.UtcNow;
            var deleted = await repository.DeleteObsoleteAsync(
                now,
                now - revokedRetention,
                cancellationToken
            );

            if (deleted > 0)
            {
                logger.LogInformation(
                    "Deleted {TokenCount} obsolete refresh tokens.",
                    deleted
                );
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Refresh-token cleanup failed and will be retried on the next interval."
            );
        }
    }

    private double GetPositiveNumber(string key, double defaultValue)
    {
        var value = configuration[key];

        return double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var parsed
            )
            && parsed > 0
            ? parsed
            : defaultValue;
    }
}
