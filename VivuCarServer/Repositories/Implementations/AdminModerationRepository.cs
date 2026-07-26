using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class AdminModerationRepository(VivuCarDbContext dbContext) : IAdminModerationRepository
{
    public async Task<IReadOnlyList<Review>> GetReviewsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Reviews.AsNoTracking()
            .Include(review => review.Customer)
            .Include(review => review.Car)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<IncidentReport>> GetIncidentReportsAsync(CancellationToken cancellationToken = default)
        => await dbContext.IncidentReports.AsNoTracking()
            .Include(report => report.Reporter)
            .OrderByDescending(report => report.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DriverDocument>> GetDriverDocumentsAsync(CancellationToken cancellationToken = default)
        => await dbContext.DriverDocuments.AsNoTracking()
            .Include(document => document.User)
            .OrderByDescending(document => document.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<DriverDocument?> GetDriverDocumentAsync(int id, CancellationToken cancellationToken = default)
        => dbContext.DriverDocuments.SingleOrDefaultAsync(document => document.Id == id, cancellationToken);

    public Task<List<Booking>> GetPendingGPLXBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => dbContext.Bookings
            .Where(b => b.CustomerId == userId && b.Status == BusinessObjects.Enums.BookingStatus.PendingGPLX)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
