using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessObjects.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.Models.Auth;

namespace Services.Implementations;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public const string TokenVersionClaim = "token_version";

    public AccessTokenResult CreateAccessToken(AppUser user)
    {
        var issuer = GetRequiredConfiguration("JWT_ISSUER");
        var audience = GetRequiredConfiguration("JWT_AUDIENCE");
        var secretKey = GetRequiredConfiguration("JWT_SECRET_KEY");
        var accessTokenMinutes = GetPositiveInteger("JWT_ACCESS_TOKEN_MINUTES");

        if (Encoding.UTF8.GetByteCount(secretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET_KEY must contain at least 32 bytes."
            );
        }

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(accessTokenMinutes);
        var jti = Guid.NewGuid().ToString("N");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(TokenVersionClaim, user.TokenVersion.ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(
                JwtRegisteredClaimNames.Iat,
                EpochTime.GetIntDate(now).ToString(),
                ClaimValueTypes.Integer64
            ),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256
        );
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            now,
            expiresAt,
            signingCredentials
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(accessToken, expiresAt, jti);
    }

    public RefreshTokenResult CreateRefreshToken()
    {
        var refreshTokenDays = GetPositiveInteger("JWT_REFRESH_TOKEN_DAYS");
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new RefreshTokenResult(
            token,
            HashRefreshToken(token),
            DateTime.UtcNow.AddDays(refreshTokenDays)
        );
    }

    public string HashRefreshToken(string token)
    {
        return Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(token))
        );
    }

    private string GetRequiredConfiguration(string key)
    {
        return configuration[key]
            ?? throw new InvalidOperationException(
                $"JWT configuration is missing. Set {key} in environment variables."
            );
    }

    private int GetPositiveInteger(string key)
    {
        var value = GetRequiredConfiguration(key);

        if (int.TryParse(value, out var result) && result > 0)
        {
            return result;
        }

        throw new InvalidOperationException($"{key} must be a positive integer.");
    }
}
