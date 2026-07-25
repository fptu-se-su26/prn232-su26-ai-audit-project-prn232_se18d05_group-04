using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class ReviewRepository(VivuCarDbContext context) : IReviewRepository
{
    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Reviews
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Review?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await context.Reviews
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(r => r.BookingId == bookingId, cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await context.Reviews.AddAsync(review, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        context.Reviews.Update(review);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Review review, CancellationToken cancellationToken = default)
    {
        context.Reviews.Remove(review);
        await context.SaveChangesAsync(cancellationToken);
    }
}
