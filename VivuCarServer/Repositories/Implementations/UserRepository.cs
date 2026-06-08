using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class UserRepository(VivuCarDbContext dbContext) : IUserRepository
{
    public Task<AppUser?> FindByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Users.SingleOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken
        );
    }

    public Task<AppUser?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Users.SingleOrDefaultAsync(
            user => user.Id == userId,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
