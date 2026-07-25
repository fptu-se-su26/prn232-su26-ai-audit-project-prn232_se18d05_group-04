using Services.Models.Booking;

namespace Services.Interfaces;

public interface IBookingService
{
    Task<bool> CheckAvailabilityAsync(int carId, DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<PricePreviewResponse> CalculatePricePreviewAsync(PricePreviewRequest request, CancellationToken cancellationToken = default);

    Task<BookingDetailResponse> CreateBookingAsync(int customerId, CreateBookingRequest request, CancellationToken cancellationToken = default);

    Task<BookingDetailResponse?> GetBookingDetailAsync(int customerId, int bookingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingDetailResponse>> GetMyBookingsAsync(int customerId, BookingListFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingDetailResponse>> GetOwnerBookingsAsync(int ownerId, BookingListFilter filter, CancellationToken cancellationToken = default);

    Task<CancelBookingResponse> CancelBookingAsync(int customerId, int bookingId, CancelBookingRequest request, CancellationToken cancellationToken = default);

    Task<BookingDetailResponse?> ApproveBookingRequestAsync(int ownerId, int bookingId, CancellationToken cancellationToken = default);

    Task<BookingDetailResponse?> RejectBookingRequestAsync(int ownerId, int bookingId, string reason, CancellationToken cancellationToken = default);

    Task<BookingDetailResponse?> GetContractAsync(int customerId, int bookingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateContractSignatureAsync(int customerId, int bookingId, string signatureUrl, CancellationToken cancellationToken = default);
}
