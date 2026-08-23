using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IOtpVerificationRepository
{
    Task<OtpVerification?> GetLatestActiveByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<OtpVerification> CreateAsync(
        OtpVerification otp,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        OtpVerification otp,
        CancellationToken cancellationToken = default
    );

    Task InvalidateAllForUserAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
