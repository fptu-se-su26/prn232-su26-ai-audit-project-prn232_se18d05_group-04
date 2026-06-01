using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Configurations;

public static class DataAccessConfiguration
{
    public static IServiceCollection AddVivuCarDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("VivuCarDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'VivuCarDatabase' is not configured.");

        services.AddDbContext<VivuCarDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
