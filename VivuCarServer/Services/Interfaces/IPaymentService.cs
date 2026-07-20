using Services.Models.Payment;

namespace Services.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentResponse> CreateDepositPaymentAsync(int customerId, CreatePaymentRequest request, CancellationToken cancellationToken = default);

    Task<PaymentStatusResponse> ProcessCallbackAsync(PaymentCallbackQuery query, CancellationToken cancellationToken = default);

    Task<PaymentStatusResponse?> GetPaymentStatusByBookingIdAsync(int customerId, int bookingId, CancellationToken cancellationToken = default);
}
