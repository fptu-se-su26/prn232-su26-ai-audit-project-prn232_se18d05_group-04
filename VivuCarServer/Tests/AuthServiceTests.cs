using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Auth;

namespace VivuCarServer.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_NormalizesEmailAndCreatesSession()
    {
        var user = CreateUser();
        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, "Customer123!");
        var dependencies = CreateDependencies(user, passwordHasher);
        var service = dependencies.CreateService();

        var result = await service.LoginAsync(
            new LoginRequest
            {
                Email = "  CUSTOMER@VIVUCAR.TEST ",
                Password = "Customer123!",
            },
            "127.0.0.1"
        );

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Response.AccessToken);
        Assert.Equal(user.Email, result.Response.User.Email);
        dependencies.UserRepository.Verify(
            repository => repository.FindByEmailAsync(
                "customer@vivucar.test",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        dependencies.RefreshTokenRepository.Verify(
            repository => repository.AddAsync(
                It.Is<RefreshToken>(token =>
                    token.UserId == user.Id
                    && token.TokenHash == "refresh-hash"
                    && token.CreatedByIp == "127.0.0.1"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Theory]
    [InlineData(UserStatus.Locked, "Customer123!")]
    [InlineData(UserStatus.Active, "wrong-password")]
    public async Task Login_RejectsLockedUserOrWrongPassword(
        UserStatus status,
        string password
    )
    {
        var user = CreateUser();
        user.Status = status;
        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, "Customer123!");
        var dependencies = CreateDependencies(user, passwordHasher);

        var result = await dependencies.CreateService().LoginAsync(
            new LoginRequest
            {
                Email = user.Email,
                Password = password,
            },
            null
        );

        Assert.Null(result);
        dependencies.TokenService.Verify(
            service => service.CreateAccessToken(It.IsAny<AppUser>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Login_RehashesPasswordWhenHasherRequestsUpgrade()
    {
        var user = CreateUser();
        var hasher = new Mock<IPasswordHasher<AppUser>>();
        hasher.Setup(service => service.VerifyHashedPassword(
                user,
                user.PasswordHash,
                "Customer123!"
            ))
            .Returns(PasswordVerificationResult.SuccessRehashNeeded);
        hasher.Setup(service => service.HashPassword(user, "Customer123!"))
            .Returns("upgraded-hash");
        var dependencies = CreateDependencies(user, hasher.Object);

        var result = await dependencies.CreateService().LoginAsync(
            new LoginRequest
            {
                Email = user.Email,
                Password = "Customer123!",
            },
            null
        );

        Assert.NotNull(result);
        Assert.Equal("upgraded-hash", user.PasswordHash);
        dependencies.UserRepository.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Refresh_WhenRotatedTokenIsReused_RevokesAllSessions()
    {
        var user = CreateUser();
        var dependencies = CreateDependencies(
            user,
            new PasswordHasher<AppUser>()
        );
        var storedToken = new RefreshToken
        {
            Id = 5,
            UserId = user.Id,
            User = user,
            TokenHash = "old-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = DateTime.UtcNow.AddMinutes(-1),
            ReplacedByTokenHash = "replacement-hash",
        };
        dependencies.TokenService
            .Setup(service => service.HashRefreshToken("old-token"))
            .Returns("old-hash");
        dependencies.RefreshTokenRepository
            .Setup(repository => repository.FindByHashAsync(
                "old-hash",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(storedToken);
        var originalVersion = user.TokenVersion;

        var result = await dependencies.CreateService().RefreshAsync(
            "old-token",
            "127.0.0.1"
        );

        Assert.Equal(RefreshSessionStatus.ReuseDetected, result.Status);
        Assert.Equal(originalVersion + 1, user.TokenVersion);
        dependencies.RefreshTokenRepository.Verify(
            repository => repository.RevokeAllActiveAsync(
                user.Id,
                It.IsAny<DateTime>(),
                "127.0.0.1",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        dependencies.SecurityStateService.Verify(
            service => service.Invalidate(user.Id),
            Times.Once
        );
    }

    private static AppUser CreateUser()
    {
        return new AppUser
        {
            Id = 12,
            Email = "customer@vivucar.test",
            FullName = "Test Customer",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            TokenVersion = 3,
            PasswordHash = "current-hash",
        };
    }

    private static TestDependencies CreateDependencies(
        AppUser user,
        IPasswordHasher<AppUser> passwordHasher
    )
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(repository => repository.FindByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(user);
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(service => service.CreateAccessToken(user))
            .Returns(
                new AccessTokenResult(
                    "access-token",
                    DateTime.UtcNow.AddMinutes(15),
                    "jti"
                )
            );
        tokenService
            .Setup(service => service.CreateRefreshToken())
            .Returns(
                new RefreshTokenResult(
                    "refresh-token",
                    "refresh-hash",
                    DateTime.UtcNow.AddDays(7)
                )
            );

        return new TestDependencies(
            userRepository,
            refreshTokenRepository,
            tokenService,
            new Mock<IUserSecurityStateService>(),
            passwordHasher
        );
    }

    private sealed record TestDependencies(
        Mock<IUserRepository> UserRepository,
        Mock<IRefreshTokenRepository> RefreshTokenRepository,
        Mock<ITokenService> TokenService,
        Mock<IUserSecurityStateService> SecurityStateService,
        IPasswordHasher<AppUser> PasswordHasher
    )
    {
        public AuthService CreateService()
        {
            return new AuthService(
                UserRepository.Object,
                RefreshTokenRepository.Object,
                PasswordHasher,
                TokenService.Object,
                SecurityStateService.Object,
                Mock.Of<ILogger<AuthService>>()
            );
        }
    }
}
