using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Review?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(Review review, CancellationToken cancellationToken = default);
}
