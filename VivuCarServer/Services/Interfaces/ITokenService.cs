using BusinessObjects.Models;
using Services.Models.Auth;

namespace Services.Interfaces;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(AppUser user);

    RefreshTokenResult CreateRefreshToken();

    string HashRefreshToken(string token);
}
