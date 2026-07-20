using Services.Models.Booking;
using Services.Models.Owner;

namespace Services.Interfaces;

public interface IOwnerBookingService
{
    Task<IReadOnlyList<OwnerBookingResponse>> GetOwnerBookingsAsync(
        int ownerId,
        OwnerBookingListFilter filter,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> GetOwnerBookingDetailAsync(
        int ownerId,
        int bookingId,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> ConfirmHandoverAsync(
        int ownerId,
        int bookingId,
        HandoverRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> ConfirmReturnAsync(
        int ownerId,
        int bookingId,
        ReturnInspectionRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> CompleteBookingAsync(
        int ownerId,
        int bookingId,
        string nextCarStatus,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> ApproveBookingAsync(
        int ownerId,
        int bookingId,
        CancellationToken cancellationToken = default
    );

    Task<OwnerBookingDetailResponse?> RejectBookingAsync(
        int ownerId,
        int bookingId,
        string? reason,
        CancellationToken cancellationToken = default
    );

    Task<OwnerDashboardStats> GetOwnerDashboardStatsAsync(
        int ownerId,
        CancellationToken cancellationToken = default
    );
}
