using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using API.Configurations;
using Services.Interfaces;
using Services.Models.Auth;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IConfiguration configuration
) : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitConfiguration.LoginPolicy)]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = await authService.LoginAsync(
            request,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );

        if (session is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        SetRefreshTokenCookie(session.RefreshToken, session.RefreshTokenExpiresAt);

        return Ok(session.Response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Refresh(
        CancellationToken cancellationToken
    )
    {
        if (
            !Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken
            )
            || string.IsNullOrWhiteSpace(refreshToken)
        )
        {
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        var result = await authService.RefreshAsync(
            refreshToken,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );

        if (
            result.Status != RefreshSessionStatus.Success
            || result.Session is null
        )
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        SetRefreshTokenCookie(
            result.Session.RefreshToken,
            result.Session.RefreshTokenExpiresAt
        );

        return Ok(result.Session.Response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(
            RefreshTokenCookieName,
            out var refreshToken
        );
        await authService.LogoutAsync(
            refreshToken,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );
        DeleteRefreshTokenCookie();

        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll(
        CancellationToken cancellationToken
    )
    {
        var userIdValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var succeeded = await authService.LogoutAllAsync(
            userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );

        if (!succeeded)
        {
            return Unauthorized();
        }

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult GetCurrentUser()
    {
        return Ok(
            new
            {
                id = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                email = User.FindFirstValue(JwtRegisteredClaimNames.Email),
                role = User.FindFirstValue(ClaimTypes.Role),
            }
        );
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("admin-check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult AdminCheck()
    {
        return Ok(new { message = "Admin access granted." });
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.RegisterAsync(request, cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> VerifyOtp(
        VerifyOtpRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.VerifyOtpAsync(
            request,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );

        if (!result.Success || result.Session is null)
        {
            return BadRequest(result);
        }

        SetRefreshTokenCookie(
            result.Session.RefreshToken,
            result.Session.RefreshTokenExpiresAt
        );

        return Ok(result.Session.Response);
    }

    private void SetRefreshTokenCookie(string token, DateTime expiresAt)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = GetBooleanConfiguration("AUTH_COOKIE_SECURE", true),
                SameSite = GetSameSiteMode(),
                Expires = new DateTimeOffset(expiresAt),
                IsEssential = true,
                Path = "/api/auth",
            }
        );
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                Secure = GetBooleanConfiguration("AUTH_COOKIE_SECURE", true),
                SameSite = GetSameSiteMode(),
                Path = "/api/auth",
            }
        );
    }

    private SameSiteMode GetSameSiteMode()
    {
        var value = configuration["AUTH_COOKIE_SAME_SITE"];

        return Enum.TryParse<SameSiteMode>(value, ignoreCase: true, out var mode)
            ? mode
            : SameSiteMode.Lax;
    }

    private bool GetBooleanConfiguration(string key, bool defaultValue)
    {
        var value = configuration[key];

        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
}
