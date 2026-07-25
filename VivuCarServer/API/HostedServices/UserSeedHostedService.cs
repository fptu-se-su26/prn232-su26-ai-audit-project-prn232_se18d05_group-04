using BusinessObjects.Data;
using BusinessObjects.Data.Seed;
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
        if (!configuration.GetValue("CloudinarySeed:Enabled", false))
        {
            logger.LogInformation("Cloudinary catalog seed skipped because CloudinarySeed:Enabled is false.");
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();
        var passwords = new Dictionary<UserRole, string>
        {
            [UserRole.Admin] = Required("SEED_ADMIN_PASSWORD"),
            [UserRole.CarOwner] = Required("SEED_CAR_OWNER_PASSWORD"),
            [UserRole.Customer] = Required("SEED_CUSTOMER_PASSWORD")
        };
        var options = new UserSeedOptions(
            Primary("ADMIN", "admin@vivucar.local", "System Admin", "0900000001"),
            Primary("CAR_OWNER", "owner01@vivucar.local", "Car Owner 01", "0900000002"),
            Primary("CUSTOMER", "customer01@vivucar.local", "Customer 01", "0900000003")
        );

        var summary = await dbContext.SeedAsync(
            options,
            user => passwordHasher.HashPassword(user, passwords[user.Role]),
            cancellationToken
        );

        var cleanup = await RemoveObsoleteLocalSeedCarsAsync(dbContext, cancellationToken);
        logger.LogInformation(
            "Cloudinary seed cleanup removed {RemovedCars} obsolete local cars and retained {RetainedCars} local cars referenced by bookings.",
            cleanup.RemovedCars,
            cleanup.RetainedCars
        );


        logger.LogInformation(
            "Application seed completed. Users +{Users}, cars +{Cars}, images +{Images}, bookings +{Bookings}, reviews +{Reviews}, payments +{Payments}, revenue snapshots +{Snapshots}.",
            summary.UsersAdded,
            summary.CarsAdded,
            summary.ImagesAdded,
            summary.BookingsAdded,
            summary.ReviewsAdded,
            summary.PaymentsAdded,
            summary.RevenueSnapshotsAdded
        );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task<(int RemovedCars, int RetainedCars)> RemoveObsoleteLocalSeedCarsAsync(
        VivuCarDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var localCarIds = await dbContext.Cars
            .Where(car => dbContext.CarImages.Any(image =>
                image.CarId == car.Id && image.ImageUrl.StartsWith("/uploads/seed/cars/")
            ))
            .Select(car => car.Id)
            .ToListAsync(cancellationToken);

        if (localCarIds.Count == 0)
        {
            return (0, 0);
        }

        var referencedCarIds = await dbContext.Bookings
            .Where(booking => localCarIds.Contains(booking.CarId))
            .Select(booking => booking.CarId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var removableCarIds = localCarIds.Except(referencedCarIds).ToArray();
        var removableCars = await dbContext.Cars
            .Where(car => removableCarIds.Contains(car.Id))
            .ToListAsync(cancellationToken);

        dbContext.Cars.RemoveRange(removableCars);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (removableCars.Count, referencedCarIds.Count);
    }
    private PrimarySeedUser Primary(
        string prefix,
        string fallbackEmail,
        string fallbackName,
        string fallbackPhone
    ) => new(
        configuration[$"{prefix}_EMAIL"]?.Trim() ?? fallbackEmail,
        configuration[$"{prefix}_FULL_NAME"]?.Trim() ?? fallbackName,
        configuration[$"{prefix}_PHONE_NUMBER"]?.Trim() ?? fallbackPhone
    );

    private string Required(string key) =>
        !string.IsNullOrWhiteSpace(configuration[key])
            ? configuration[key]!
            : throw new InvalidOperationException($"Seed configuration '{key}' is required.");
}