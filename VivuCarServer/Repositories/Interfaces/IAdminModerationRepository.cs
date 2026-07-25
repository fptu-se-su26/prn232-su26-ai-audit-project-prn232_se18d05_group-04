using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IAdminModerationRepository
{
    Task<IReadOnlyList<Review>> GetReviewsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidentReport>> GetIncidentReportsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DriverDocument>> GetDriverDocumentsAsync(CancellationToken cancellationToken = default);
    Task<DriverDocument?> GetDriverDocumentAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetPendingApprovalBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
