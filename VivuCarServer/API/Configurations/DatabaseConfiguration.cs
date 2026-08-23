using System.Text.RegularExpressions;
using BusinessObjects.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddVivuCarDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = BuildConnectionString(configuration);

        return services.AddDbContext<VivuCarDbContext>(options =>
            options.UseSqlServer(connectionString)
        );
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var connectionString =
            configuration["DB_CONNECTION_STRING"]
            ?? configuration.GetConnectionString("VivuCarDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'VivuCarDatabase' is not configured."
            );
        }

        return ResolveEnvironmentPlaceholders(configuration, connectionString);
    }

    private static string ResolveEnvironmentPlaceholders(
        IConfiguration configuration,
        string connectionString
    )
    {
        return Regex.Replace(
            connectionString,
            @"\$\{(?<key>[A-Z0-9_]+)\}",
            match =>
            {
                var key = match.Groups["key"].Value;
                var value = configuration[key];

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                throw new InvalidOperationException(
                    $"Database configuration is missing. Set {key} in environment variables."
                );
            }
        );
    }
}
