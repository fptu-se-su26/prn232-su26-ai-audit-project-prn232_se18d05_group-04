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
        services.AddHostedService<API.HostedServices.BookingExpirationHostedService>();

        return services;
    }
}
