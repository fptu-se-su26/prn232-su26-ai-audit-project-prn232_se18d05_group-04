using BusinessObjects.Enums;
using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;

namespace VivuCarServer.Tests;

public class AdminUserServiceContractTests
{
    [Fact]
    public async Task GetUsersAsync_MapsDatabaseEnumsToPublicSchemaContract()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([
            new AppUser { Id = 1, Email = "renter@vivucar.vn", FullName = "Renter", Role = UserRole.Customer, Status = UserStatus.Active },
            new AppUser { Id = 2, Email = "owner@vivucar.vn", FullName = "Owner", Role = UserRole.CarOwner, Status = UserStatus.Locked }
        ]);
        var service = new AdminUserService(users.Object, Mock.Of<IRefreshTokenRepository>(), Mock.Of<IUserSecurityStateService>());

        var result = await service.GetUsersAsync();

        Assert.Equal("user", result[0].Role);
        Assert.False(result[0].IsBlocked);
        Assert.Equal("car_owner", result[1].Role);
        Assert.True(result[1].IsBlocked);
    }
}
