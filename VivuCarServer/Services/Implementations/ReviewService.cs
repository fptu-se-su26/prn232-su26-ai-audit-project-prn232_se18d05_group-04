using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Review;

namespace Services.Implementations;

public class ReviewService(IReviewRepository reviewRepository, IBookingRepository bookingRepository) : IReviewService
{
    public async Task<ReviewDetailResponse?> GetByBookingIdAsync(int customerId, int bookingId, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByBookingIdAsync(bookingId, cancellationToken);
        if (review == null) return null;
        
        if (review.CustomerId != customerId && review.Booking.Car.OwnerId != customerId)
        {
            return null; // Return null if user is not authorized
        }

        return MapToDetailResponse(review);
    }

    public async Task<ReviewDetailResponse> CreateReviewAsync(int customerId, ReviewCreateRequest request, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new ReviewServiceException(404, "Booking not found.");
        }

        if (booking.CustomerId != customerId)
        {
            throw new ReviewServiceException(403, "You can only review your own bookings.");
        }

        if (booking.Status != BookingStatus.Completed)
        {
            throw new ReviewServiceException(400, "You can only leave a review after the rental process is complete.");
        }

        var existingReview = await reviewRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existingReview != null)
        {
            throw new ReviewServiceException(409, "A review for this booking already exists.");
        }

        var review = new Review
        {
            BookingId = request.BookingId,
            CarId = booking.CarId,
            CustomerId = customerId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await reviewRepository.AddAsync(review, cancellationToken);

        return MapToDetailResponse(review);
    }

    public async Task<ReviewDetailResponse> UpdateReviewAsync(int customerId, int reviewId, ReviewUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new ReviewServiceException(404, "Review not found.");
        }

        if (review.CustomerId != customerId)
        {
            throw new ReviewServiceException(403, "You can only edit your own reviews.");
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment;

        await reviewRepository.UpdateAsync(review, cancellationToken);

        return MapToDetailResponse(review);
    }

    public async Task DeleteReviewAsync(int customerId, int reviewId, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new ReviewServiceException(404, "Review not found.");
        }

        if (review.CustomerId != customerId)
        {
            throw new ReviewServiceException(403, "You can only delete your own reviews.");
        }

        await reviewRepository.DeleteAsync(review, cancellationToken);
    }

    private static ReviewDetailResponse MapToDetailResponse(Review review)
    {
        return new ReviewDetailResponse
        {
            Id = review.Id,
            BookingId = review.BookingId,
            CarId = review.CarId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            CustomerId = review.CustomerId
        };
    }
}
