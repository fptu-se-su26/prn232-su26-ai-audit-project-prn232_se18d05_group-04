using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Auth;

namespace Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher<AppUser> passwordHasher,
    ITokenService tokenService,
    IUserSecurityStateService userSecurityStateService,
    ILogger<AuthService> logger,
    IOtpVerificationRepository otpRepository,
    IOtpService otpService,
    IEmailService emailService
) : IAuthService
{
    public async Task<AuthSessionResult?> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || user.Status != UserStatus.Active)
        {
            return null;
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            user.UpdatedAt = DateTime.UtcNow;
            await userRepository.SaveChangesAsync(cancellationToken);
        }

        return await CreateSessionAsync(user, ipAddress, cancellationToken);
    }

    public async Task<RefreshSessionResult> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var tokenHash = tokenService.HashRefreshToken(refreshToken);
        var storedToken = await refreshTokenRepository.FindByHashAsync(
            tokenHash,
            cancellationToken
        );

        if (storedToken is null)
        {
            return new RefreshSessionResult(RefreshSessionStatus.Invalid);
        }

        if (storedToken.RevokedAt is not null)
        {
            if (!string.IsNullOrWhiteSpace(storedToken.ReplacedByTokenHash))
            {
                await HandleRefreshTokenReuseAsync(
                    storedToken.User,
                    ipAddress,
                    cancellationToken
                );

                return new RefreshSessionResult(
                    RefreshSessionStatus.ReuseDetected
                );
            }

            return new RefreshSessionResult(RefreshSessionStatus.Invalid);
        }

        if (
            storedToken.ExpiresAt <= DateTime.UtcNow
            || storedToken.User.Status != UserStatus.Active
        )
        {
            return new RefreshSessionResult(RefreshSessionStatus.Invalid);
        }

        var accessToken = tokenService.CreateAccessToken(storedToken.User);
        var replacement = tokenService.CreateRefreshToken();
        var replacementEntity = CreateRefreshTokenEntity(
            storedToken.UserId,
            replacement,
            ipAddress
        );
        var rotated = await refreshTokenRepository.TryRotateAsync(
            storedToken,
            replacementEntity,
            DateTime.UtcNow,
            ipAddress,
            cancellationToken
        );

        if (!rotated)
        {
            await HandleRefreshTokenReuseAsync(
                storedToken.User,
                ipAddress,
                cancellationToken
            );

            return new RefreshSessionResult(RefreshSessionStatus.ReuseDetected);
        }

        var response = CreateLoginResponse(storedToken.User, accessToken);

        return new RefreshSessionResult(
            RefreshSessionStatus.Success,
            new AuthSessionResult(
                response,
                replacement.Token,
                replacement.ExpiresAt
            )
        );
    }

    public async Task LogoutAsync(
        string? refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        await refreshTokenRepository.RevokeByHashAsync(
            tokenService.HashRefreshToken(refreshToken),
            DateTime.UtcNow,
            ipAddress,
            cancellationToken
        );
    }

    public async Task<bool> LogoutAllAsync(
        int userId,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        await refreshTokenRepository.RevokeAllActiveAsync(
            userId,
            DateTime.UtcNow,
            ipAddress,
            cancellationToken
        );
        user.TokenVersion++;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(userId);

        return true;
    }

    public async Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingUser = await userRepository.FindByEmailAsync(
            normalizedEmail,
            cancellationToken
        );

        if (existingUser is not null)
        {
            return new RegisterResult(false, "Email đã được đăng ký.");
        }

        var passwordHash = passwordHasher.HashPassword(
            new AppUser(),
            request.Password
        );

        var user = new AppUser
        {
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            TokenVersion = 1,
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        // Generate OTP and send email
        string? otpError = null;
        try
        {
            await SendOtpForUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to send OTP email for user {UserId} ({Email})",
                user.Id,
                user.Email
            );
            otpError = $"Không gửi được email OTP: {ex.Message}";
        }

        var message = otpError
            ?? "Đăng ký thành công. Vui lòng kiểm tra email để xác thực.";
        return new RegisterResult(true, message, user.Id);
    }

    public async Task<VerifyOtpResult> VerifyOtpAsync(
        VerifyOtpRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return new VerifyOtpResult(false, "Email không tồn tại.");
        }

        var otp = await otpRepository.GetLatestActiveByUserIdAsync(user.Id, cancellationToken);

        if (otp is null)
        {
            return new VerifyOtpResult(false, "Không tìm thấy mã OTP. Vui lòng yêu cầu mã mới.");
        }

        if (otp.AttemptCount >= 5)
        {
            otp.IsUsed = true;
            await otpRepository.UpdateAsync(otp, cancellationToken);
            return new VerifyOtpResult(false, "Đã vượt quá số lần thử. Vui lòng đăng ký lại.");
        }

        if (otpService.IsExpired(otp.ExpiredAt))
        {
            otp.IsUsed = true;
            await otpRepository.UpdateAsync(otp, cancellationToken);
            return new VerifyOtpResult(false, "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới.");
        }

        otp.AttemptCount++;
        await otpRepository.UpdateAsync(otp, cancellationToken);

        if (!string.Equals(otp.Code, request.Code, StringComparison.Ordinal))
        {
            var remaining = 5 - otp.AttemptCount;
            return new VerifyOtpResult(false, $"Mã OTP không đúng. Còn {remaining} lần thử.");
        }

        otp.IsUsed = true;
        await otpRepository.UpdateAsync(otp, cancellationToken);

        var session = await CreateSessionAsync(user, ipAddress, cancellationToken);
        return new VerifyOtpResult(true, "Xác thực thành công.", session);
    }

    public async Task<RegisterResult> ResendOtpAsync(
        ResendOtpRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return new RegisterResult(false, "Email không tồn tại.");
        }

        try
        {
            await SendOtpForUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to resend OTP email for user {UserId} ({Email})",
                user.Id,
                user.Email
            );
            return new RegisterResult(false, $"Không gửi được email OTP: {ex.Message}");
        }

        return new RegisterResult(true, "Đã gửi lại mã OTP. Vui lòng kiểm tra email.");
    }

    private async Task SendOtpForUserAsync(AppUser user, CancellationToken cancellationToken)
    {
        await otpRepository.InvalidateAllForUserAsync(user.Id, cancellationToken);

        var otp = new OtpVerification
        {
            UserId = user.Id,
            Code = otpService.GenerateCode(),
            ExpiredAt = otpService.GetExpiryTime(),
        };

        await otpRepository.CreateAsync(otp, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        await emailService.SendOtpEmailAsync(user.Email, otp.Code);
    }

    private async Task<AuthSessionResult> CreateSessionAsync(
        AppUser user,
        string? ipAddress,
        CancellationToken cancellationToken
    )
    {
        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();
        var refreshTokenEntity = CreateRefreshTokenEntity(
            user.Id,
            refreshToken,
            ipAddress
        );

        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult(
            CreateLoginResponse(user, accessToken),
            refreshToken.Token,
            refreshToken.ExpiresAt
        );
    }

    private async Task HandleRefreshTokenReuseAsync(
        AppUser user,
        string? ipAddress,
        CancellationToken cancellationToken
    )
    {
        logger.LogWarning(
            "Refresh-token reuse was detected for user {UserId}; all sessions are being revoked.",
            user.Id
        );
        await refreshTokenRepository.RevokeAllActiveAsync(
            user.Id,
            DateTime.UtcNow,
            ipAddress,
            cancellationToken
        );
        user.TokenVersion++;
        await userRepository.SaveChangesAsync(cancellationToken);
        userSecurityStateService.Invalidate(user.Id);
    }

    private static RefreshToken CreateRefreshTokenEntity(
        int userId,
        RefreshTokenResult token,
        string? ipAddress
    )
    {
        return new RefreshToken
        {
            UserId = userId,
            TokenHash = token.TokenHash,
            ExpiresAt = token.ExpiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
        };
    }

    private static LoginResponse CreateLoginResponse(
        AppUser user,
        AccessTokenResult token
    )
    {
        return new LoginResponse(
            token.AccessToken,
            token.ExpiresAt,
            new AuthenticatedUserResponse(
                user.Id,
                user.Email,
                user.FullName,
                user.Role.ToString()
            )
        );
    }
}
