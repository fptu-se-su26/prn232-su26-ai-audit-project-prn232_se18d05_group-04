using API.Services.Storage;
using Services.Implementations;
using Services.Interfaces;

namespace API.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddVivuCarServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserSecurityStateService, UserSecurityStateService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IAdminCarService, AdminCarService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICarService, CarService>();
        services.AddHostedService<API.HostedServices.BookingExpirationHostedService>();
        services.AddHostedService<API.HostedServices.MiotoCarSeedHostedService>();

        return services;
    }
}


