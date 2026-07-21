using API.Services.Storage;
using Services.Implementations;
using Services.Interfaces;

namespace API.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddVivuCarServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        // Register PayOS singleton — required by PaymentService via DI
        var payOsClientId = Environment.GetEnvironmentVariable("PayOS__ClientId") ?? "";
        var payOsApiKey = Environment.GetEnvironmentVariable("PayOS__ApiKey") ?? "";
        var payOsChecksumKey = Environment.GetEnvironmentVariable("PayOS__ChecksumKey") ?? "";
        services.AddSingleton(new Net.payOS.PayOS(payOsClientId, payOsApiKey, payOsChecksumKey));

        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserSecurityStateService, UserSecurityStateService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IAdminCarService, AdminCarService>();
        services.AddScoped<IAdminVoucherService, AdminVoucherService>();
        services.AddScoped<IAdminReportService, AdminReportService>();
        services.AddScoped<IFileStorageService, CloudinaryStorageService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IOwnerCarService, OwnerCarService>();
        services.AddScoped<IOwnerBookingService, OwnerBookingService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddHostedService<API.HostedServices.BookingExpirationHostedService>();
        services.AddHostedService<API.HostedServices.AppDbSeederHostedService>();
        services.AddHostedService<API.HostedServices.MiotoCarSeedHostedService>();

        return services;
    }
}


