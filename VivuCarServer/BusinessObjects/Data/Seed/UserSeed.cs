using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data.Seed;

public sealed record PrimarySeedUser(
    string Email,
    string FullName,
    string PhoneNumber
);

public sealed record UserSeedOptions(
    PrimarySeedUser Admin,
    PrimarySeedUser CarOwner,
    PrimarySeedUser Customer
);

public static class UserSeed
{
    public const int AdminCount = 2;
    public const int CarOwnerCount = 10;
    public const int CustomerCount = 20;

    public static async Task<int> SeedAsync(
        VivuCarDbContext dbContext,
        UserSeedOptions options,
        Func<AppUser, string> passwordHashFactory,
        CancellationToken cancellationToken = default
    )
    {
        var definitions = BuildDefinitions(options);
        var emails = definitions.Select(item => item.Email).ToArray();
        var existingEmails = await dbContext.Users
            .Where(user => emails.Contains(user.Email))
            .Select(user => user.Email)
            .ToListAsync(cancellationToken);
        var existing = existingEmails.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var added = 0;

        foreach (var definition in definitions)
        {
            if (existing.Contains(definition.Email)) continue;

            var user = new AppUser
            {
                Email = definition.Email,
                FullName = definition.FullName,
                PhoneNumber = definition.PhoneNumber,
                Role = definition.Role,
                Status = UserStatus.Active,
                TokenVersion = 1,
                DateOfBirth = definition.DateOfBirth,
                Address = definition.Address,
                CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(added)
            };
            user.PasswordHash = passwordHashFactory(user);
            dbContext.Users.Add(user);
            existing.Add(user.Email);
            added++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return added;
    }

    private static IReadOnlyList<UserDefinition> BuildDefinitions(UserSeedOptions options)
    {
        var users = new List<UserDefinition>(AdminCount + CarOwnerCount + CustomerCount)
        {
            FromPrimary(options.Admin, UserRole.Admin, new DateOnly(1990, 4, 12), "Hai Chau, Da Nang"),
            FromPrimary(options.CarOwner, UserRole.CarOwner, new DateOnly(1988, 9, 21), "Son Tra, Da Nang"),
            FromPrimary(options.Customer, UserRole.Customer, new DateOnly(1998, 2, 15), "Thanh Khe, Da Nang")
        };

        for (var index = 2; index <= AdminCount; index++)
        {
            users.Add(new UserDefinition(
                $"admin{index:00}@vivucar.local",
                $"VivuCar Admin {index:00}",
                $"090100{index:0000}",
                UserRole.Admin,
                new DateOnly(1989, 1, Math.Min(index + 4, 28)),
                "Hai Chau, Da Nang"
            ));
        }

        for (var index = 2; index <= CarOwnerCount; index++)
        {
            users.Add(new UserDefinition(
                $"owner{index:00}@vivucar.local",
                $"Car Owner {index:00}",
                $"090200{index:0000}",
                UserRole.CarOwner,
                new DateOnly(1985 + index % 10, 3 + index % 8, Math.Min(index + 5, 28)),
                OwnerAddress(index)
            ));
        }

        for (var index = 2; index <= CustomerCount; index++)
        {
            users.Add(new UserDefinition(
                $"customer{index:00}@vivucar.local",
                $"Customer {index:00}",
                $"090300{index:0000}",
                UserRole.Customer,
                new DateOnly(1992 + index % 10, 1 + index % 11, Math.Min(index + 3, 28)),
                CustomerAddress(index)
            ));
        }

        return users;
    }

    private static UserDefinition FromPrimary(
        PrimarySeedUser user,
        UserRole role,
        DateOnly dateOfBirth,
        string address
    ) => new(
        user.Email.Trim().ToLowerInvariant(),
        user.FullName.Trim(),
        user.PhoneNumber.Trim(),
        role,
        dateOfBirth,
        address
    );

    private static string OwnerAddress(int index) => (index % 4) switch
    {
        0 => "Ngu Hanh Son, Da Nang",
        1 => "Son Tra, Da Nang",
        2 => "Cam Le, Da Nang",
        _ => "Lien Chieu, Da Nang"
    };

    private static string CustomerAddress(int index) => (index % 4) switch
    {
        0 => "Hai Chau, Da Nang",
        1 => "Thanh Khe, Da Nang",
        2 => "Son Tra, Da Nang",
        _ => "Ngu Hanh Son, Da Nang"
    };

    private sealed record UserDefinition(
        string Email,
        string FullName,
        string PhoneNumber,
        UserRole Role,
        DateOnly DateOfBirth,
        string Address
    );
}