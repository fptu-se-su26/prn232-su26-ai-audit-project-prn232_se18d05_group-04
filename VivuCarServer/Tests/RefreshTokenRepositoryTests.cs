using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementations;

namespace VivuCarServer.Tests;

public class RefreshTokenRepositoryTests
{
    [Fact]
    public async Task DeleteObsolete_RemovesExpiredAndOldRevokedOnly()
    {
        await using var fixture = await DatabaseFixture.CreateAsync();
        var now = DateTime.UtcNow;
        fixture.Context.RefreshTokens.AddRange(
            CreateToken("active", now.AddDays(1)),
            CreateToken("expired", now.AddMinutes(-1)),
            CreateToken("old-revoked", now.AddDays(1), now.AddDays(-8)),
            CreateToken("recent-revoked", now.AddDays(1), now.AddDays(-1))
        );
        await fixture.Context.SaveChangesAsync();
        var repository = new RefreshTokenRepository(fixture.Context);

        var deleted = await repository.DeleteObsoleteAsync(
            now,
            now.AddDays(-7)
        );
        var remainingHashes = await fixture.Context.RefreshTokens
            .OrderBy(token => token.TokenHash)
            .Select(token => token.TokenHash)
            .ToListAsync();

        Assert.Equal(2, deleted);
        Assert.Equal(["active", "recent-revoked"], remainingHashes);
    }

    [Fact]
    public async Task TryRotate_RevokesCurrentAndCreatesReplacementOnce()
    {
        await using var fixture = await DatabaseFixture.CreateAsync();
        var current = CreateToken("current", DateTime.UtcNow.AddDays(1));
        fixture.Context.RefreshTokens.Add(current);
        await fixture.Context.SaveChangesAsync();
        var repository = new RefreshTokenRepository(fixture.Context);
        var replacement = CreateToken(
            "replacement",
            DateTime.UtcNow.AddDays(1)
        );

        var firstRotation = await repository.TryRotateAsync(
            current,
            replacement,
            DateTime.UtcNow,
            "127.0.0.1"
        );
        var secondRotation = await repository.TryRotateAsync(
            current,
            CreateToken("another", DateTime.UtcNow.AddDays(1)),
            DateTime.UtcNow,
            "127.0.0.1"
        );

        Assert.True(firstRotation);
        Assert.False(secondRotation);
        Assert.Equal(2, await fixture.Context.RefreshTokens.CountAsync());
        fixture.Context.ChangeTracker.Clear();
        var storedCurrent = await fixture.Context.RefreshTokens
            .SingleAsync(token => token.TokenHash == "current");
        Assert.NotNull(storedCurrent.RevokedAt);
        Assert.Equal("replacement", storedCurrent.ReplacedByTokenHash);
    }

    private static RefreshToken CreateToken(
        string hash,
        DateTime expiresAt,
        DateTime? revokedAt = null
    )
    {
        return new RefreshToken
        {
            UserId = 1,
            TokenHash = hash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            RevokedAt = revokedAt,
        };
    }

    private sealed class DatabaseFixture : IAsyncDisposable
    {
        private readonly SqliteConnection connection;

        public VivuCarDbContext Context { get; }

        private DatabaseFixture(
            SqliteConnection connection,
            VivuCarDbContext context
        )
        {
            this.connection = connection;
            Context = context;
        }

        public static async Task<DatabaseFixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<VivuCarDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new VivuCarDbContext(options);
            await context.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    AvatarUrl TEXT NULL,
                    Role TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    TokenVersion INTEGER NOT NULL,
                    DateOfBirth TEXT NULL,
                    Address TEXT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NULL
                );

                CREATE TABLE RefreshTokens (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    TokenHash TEXT NOT NULL,
                    ExpiresAt TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    RevokedAt TEXT NULL,
                    ReplacedByTokenHash TEXT NULL,
                    CreatedByIp TEXT NULL,
                    RevokedByIp TEXT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE UNIQUE INDEX IX_RefreshTokens_TokenHash
                    ON RefreshTokens(TokenHash);
                """
            );
            context.Users.Add(
                new AppUser
                {
                    Id = 1,
                    Email = "owner@vivucar.test",
                    PasswordHash = "hash",
                    FullName = "Owner",
                    PhoneNumber = "0900000000",
                    Role = UserRole.CarOwner,
                    Status = UserStatus.Active,
                    TokenVersion = 1,
                    CreatedAt = DateTime.UtcNow,
                }
            );
            await context.SaveChangesAsync();

            return new DatabaseFixture(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await connection.DisposeAsync();
        }
    }
}
