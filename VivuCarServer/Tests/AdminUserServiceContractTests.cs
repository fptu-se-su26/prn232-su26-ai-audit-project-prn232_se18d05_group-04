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
            new AppUser { Id = 2, Email = "owner@vivucar.vn", FullName = "Owner", Role = UserRole.CarOwner, Status = UserStatus.Locked },
            new AppUser { Id = 3, Email = "deleted@vivucar.vn", FullName = "Deleted", Role = UserRole.Customer, Status = UserStatus.Deleted }
        ]);
        var service = new AdminUserService(users.Object, Mock.Of<IRefreshTokenRepository>(), Mock.Of<IUserSecurityStateService>());

        var result = await service.GetUsersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("user", result[0].Role);
        Assert.False(result[0].IsBlocked);
        Assert.Equal("car_owner", result[1].Role);
        Assert.True(result[1].IsBlocked);
    }

    [Fact]
    public async Task SoftDeleteCustomerAsync_MarksCustomerDeletedAndRevokesSession()
    {
        var customer = new AppUser
        {
            Id = 9,
            Email = "customer@vivucar.vn",
            FullName = "Customer",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            TokenVersion = 1
        };
        var users = new Mock<IUserRepository>();
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        var securityState = new Mock<IUserSecurityStateService>();
        users
            .Setup(x => x.FindByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var service = new AdminUserService(
            users.Object,
            refreshTokens.Object,
            securityState.Object
        );

        var result = await service.SoftDeleteCustomerAsync(
            customer.Id,
            "127.0.0.1"
        );

        Assert.True(result);
        Assert.Equal(UserStatus.Deleted, customer.Status);
        Assert.Equal(2, customer.TokenVersion);
        Assert.NotNull(customer.UpdatedAt);
        refreshTokens.Verify(
            x => x.RevokeAllActiveAsync(
                customer.Id,
                It.IsAny<DateTime>(),
                "127.0.0.1",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        users.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        securityState.Verify(x => x.Invalidate(customer.Id), Times.Once);
    }

    [Fact]
    public async Task GetDeletedCustomersAsync_ReturnsOnlyDeletedCustomers()
    {
        var deletedAt = new DateTime(2026, 7, 21, 8, 30, 0, DateTimeKind.Utc);
        var users = new Mock<IUserRepository>();
        users
            .Setup(x => x.GetByStatusAsync(
                UserStatus.Deleted,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync([
                new AppUser
                {
                    Id = 12,
                    Email = "deleted@vivucar.vn",
                    FullName = "Deleted Customer",
                    Role = UserRole.Customer,
                    Status = UserStatus.Deleted,
                    CreatedAt = deletedAt.AddDays(-10),
                    UpdatedAt = deletedAt
                },
                new AppUser
                {
                    Id = 13,
                    Email = "admin@vivucar.vn",
                    FullName = "Deleted Admin",
                    Role = UserRole.Admin,
                    Status = UserStatus.Deleted,
                    CreatedAt = deletedAt.AddDays(-20),
                    UpdatedAt = deletedAt
                }
            ]);
        var service = new AdminUserService(
            users.Object,
            Mock.Of<IRefreshTokenRepository>(),
            Mock.Of<IUserSecurityStateService>()
        );

        var result = await service.GetDeletedCustomersAsync();

        var item = Assert.Single(result);
        Assert.Equal("users", item.Type);
        Assert.Equal(12, item.Id);
        Assert.Equal("Deleted Customer", item.Name);
        Assert.Equal("deleted@vivucar.vn", item.Info);
        Assert.Equal(deletedAt, item.DeletedAt);
    }

    [Fact]
    public async Task RestoreCustomerAsync_ReactivatesDeletedCustomer()
    {
        var customer = new AppUser
        {
            Id = 14,
            Email = "restore@vivucar.vn",
            FullName = "Restore Customer",
            Role = UserRole.Customer,
            Status = UserStatus.Deleted,
            TokenVersion = 3
        };
        var users = new Mock<IUserRepository>();
        var securityState = new Mock<IUserSecurityStateService>();
        users
            .Setup(x => x.FindByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        var service = new AdminUserService(
            users.Object,
            Mock.Of<IRefreshTokenRepository>(),
            securityState.Object
        );

        var result = await service.RestoreCustomerAsync(customer.Id);

        Assert.True(result);
        Assert.Equal(UserStatus.Active, customer.Status);
        Assert.Equal(4, customer.TokenVersion);
        Assert.NotNull(customer.UpdatedAt);
        users.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        securityState.Verify(x => x.Invalidate(customer.Id), Times.Once);
    }
}
