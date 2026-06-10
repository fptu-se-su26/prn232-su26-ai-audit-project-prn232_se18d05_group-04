using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.HostedServices;

public class UserSeedHostedService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<UserSeedHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_ADMIN",
            UserRole.Admin,
            "System Admin",
            "0900000001",
            cancellationToken
        );
        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_CUSTOMER",
            UserRole.Customer,
            "Default Customer",
            "0900000002",
            cancellationToken
        );
        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_CAR_OWNER",
            UserRole.CarOwner,
            "Default Car Owner",
            "0900000003",
            cancellationToken
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedUserAsync(
        VivuCarDbContext dbContext,
        IPasswordHasher<AppUser> passwordHasher,
        string configurationPrefix,
        UserRole role,
        string defaultFullName,
        string defaultPhoneNumber,
        CancellationToken cancellationToken
    )
    {
        var email = configuration[$"{configurationPrefix}_EMAIL"]?.Trim().ToLowerInvariant();
        var password = configuration[$"{configurationPrefix}_PASSWORD"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "{Role} seed account was skipped because {EmailKey} or {PasswordKey} is missing.",
                role,
                $"{configurationPrefix}_EMAIL",
                $"{configurationPrefix}_PASSWORD"
            );
            return;
        }

        var existingUser = await dbContext.Users.SingleOrDefaultAsync(
            user => user.Email == email,
            cancellationToken
        );

        if (existingUser is not null)
        {
            if (existingUser.PasswordHash.StartsWith("SeedPasswordHash_", StringComparison.Ordinal))
            {
                existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);
                existingUser.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Legacy placeholder password hash was upgraded for {Email}.",
                    email
                );
            }

            return;
        }

        var user = new AppUser
        {
            Email = email,
            FullName =
                configuration[$"{configurationPrefix}_FULL_NAME"]?.Trim() ?? defaultFullName,
            PhoneNumber =
                configuration[$"{configurationPrefix}_PHONE_NUMBER"]?.Trim()
                ?? defaultPhoneNumber,
            Role = role,
            Status = UserStatus.Active,
            TokenVersion = 1,
            CreatedAt = DateTime.UtcNow,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Default {Role} account {Email} was seeded.", role, email);
    }
}
