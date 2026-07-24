using System.Threading;
using System.Threading.Tasks;
using Services.Models.Review;

namespace Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDetailResponse?> GetByBookingIdAsync(int customerId, int bookingId, CancellationToken cancellationToken = default);
    Task<ReviewDetailResponse> CreateReviewAsync(int customerId, ReviewCreateRequest request, CancellationToken cancellationToken = default);
    Task<ReviewDetailResponse> UpdateReviewAsync(int customerId, int reviewId, ReviewUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteReviewAsync(int customerId, int reviewId, CancellationToken cancellationToken = default);
}

public class ReviewServiceException(int statusCode, string message) : System.Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
