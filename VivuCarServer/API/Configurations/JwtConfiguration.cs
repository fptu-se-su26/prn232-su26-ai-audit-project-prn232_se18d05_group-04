
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.Implementations;
using Services.Interfaces;

namespace API.Configurations;

public static class JwtConfiguration
{
    public static IServiceCollection AddVivuCarJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var issuer = GetRequiredConfiguration(configuration, "JWT_ISSUER");
        var audience = GetRequiredConfiguration(configuration, "JWT_AUDIENCE");
        var secretKey = GetRequiredConfiguration(configuration, "JWT_SECRET_KEY");

        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET_KEY must contain at least 32 bytes."
            );
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)
                    ),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role,
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ValidateUserSecurityStateAsync,
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static async Task ValidateUserSecurityStateAsync(
        TokenValidatedContext context
    )
    {
        var userIdValue = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var tokenVersionValue = context.Principal?.FindFirstValue(
            JwtTokenService.TokenVersionClaim
        );
        var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);

        if (
            !int.TryParse(userIdValue, out var userId)
            || !int.TryParse(tokenVersionValue, out var tokenVersion)
            || string.IsNullOrWhiteSpace(jti)
        )
        {
            context.Fail("Required token claims are missing.");
            return;
        }

        var securityStateService = context.HttpContext.RequestServices
            .GetRequiredService<IUserSecurityStateService>();
        var state = await securityStateService.GetAsync(
            userId,
            context.HttpContext.RequestAborted
        );

        if (
            state is null
            || state.Status != UserStatus.Active
            || state.TokenVersion != tokenVersion
        )
        {
            context.Fail("The user session is no longer valid.");
        }
    }

    private static string GetRequiredConfiguration(
        IConfiguration configuration,
        string key
    )
    {
        return configuration[key]
            ?? throw new InvalidOperationException(
                $"JWT configuration is missing. Set {key} in environment variables."
            );
    }
}
