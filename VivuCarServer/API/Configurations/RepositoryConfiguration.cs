using Repositories.Implementations;
using Repositories.Interfaces;

namespace API.Configurations;

public static class RepositoryConfiguration
{
    public static IServiceCollection AddVivuCarRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ICarRepository, CarRepository>();

        return services;
    }
}
