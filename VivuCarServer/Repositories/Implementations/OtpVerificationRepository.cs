using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class OtpVerificationRepository(VivuCarDbContext dbContext) : IOtpVerificationRepository
{
    public Task<OtpVerification?> GetLatestActiveByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return dbContext.Set<OtpVerification>()
            .Where(otp => otp.UserId == userId && !otp.IsUsed)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OtpVerification> CreateAsync(
        OtpVerification otp,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await dbContext.Set<OtpVerification>().AddAsync(otp, cancellationToken);
        return entry.Entity;
    }

    public Task UpdateAsync(
        OtpVerification otp,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.Set<OtpVerification>().Update(otp);
        return Task.CompletedTask;
    }

    public async Task InvalidateAllForUserAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.Set<OtpVerification>()
            .Where(otp => otp.UserId == userId && !otp.IsUsed)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(otp => otp.IsUsed, true),
                cancellationToken
            );
    }
}
