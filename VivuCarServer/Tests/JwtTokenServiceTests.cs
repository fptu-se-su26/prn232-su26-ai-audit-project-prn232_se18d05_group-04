using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Implementations;

namespace VivuCarServer.Tests;

public class JwtTokenServiceTests
{
    private const string Issuer = "VivuCarServer.Tests";
    private const string Audience = "VivuCarClient.Tests";
    private const string Secret = "test-secret-key-with-at-least-32-characters";

    [Fact]
    public void CreateAccessToken_EmitsValidExpectedClaims()
    {
        var service = CreateService();
        var user = new AppUser
        {
            Id = 42,
            Email = "customer@vivucar.test",
            Role = UserRole.Customer,
            TokenVersion = 7,
        };

        var result = service.CreateAccessToken(user);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Secret)
            ),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
        };
        var handler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false,
        };

        var principal = handler.ValidateToken(
            result.AccessToken,
            parameters,
            out _
        );

        Assert.Equal("42", principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        Assert.Equal(
            user.Email,
            principal.FindFirstValue(JwtRegisteredClaimNames.Email)
        );
        Assert.Equal(
            UserRole.Customer.ToString(),
            principal.FindFirstValue(ClaimTypes.Role)
        );
        Assert.Equal(
            "7",
            principal.FindFirstValue(JwtTokenService.TokenVersionClaim)
        );
        Assert.Equal(result.Jti, principal.FindFirstValue(JwtRegisteredClaimNames.Jti));
        Assert.InRange(
            result.ExpiresAt,
            DateTime.UtcNow.AddMinutes(14),
            DateTime.UtcNow.AddMinutes(16)
        );
    }

    [Fact]
    public void CreateRefreshToken_ReturnsRandomTokenAndStableHash()
    {
        var service = CreateService();

        var first = service.CreateRefreshToken();
        var second = service.CreateRefreshToken();

        Assert.NotEqual(first.Token, second.Token);
        Assert.NotEqual(first.Token, first.TokenHash);
        Assert.Equal(first.TokenHash, service.HashRefreshToken(first.Token));
        Assert.Equal(44, first.TokenHash.Length);
    }

    [Fact]
    public void CreateAccessToken_RejectsShortSecret()
    {
        var service = CreateService(secret: "too-short");

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateAccessToken(new AppUser())
        );

        Assert.Contains("at least 32 bytes", exception.Message);
    }

    private static JwtTokenService CreateService(string secret = Secret)
    {
        var values = new Dictionary<string, string?>
        {
            ["JWT_ISSUER"] = Issuer,
            ["JWT_AUDIENCE"] = Audience,
            ["JWT_SECRET_KEY"] = secret,
            ["JWT_ACCESS_TOKEN_MINUTES"] = "15",
            ["JWT_REFRESH_TOKEN_DAYS"] = "7",
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        return new JwtTokenService(configuration);
    }
}
