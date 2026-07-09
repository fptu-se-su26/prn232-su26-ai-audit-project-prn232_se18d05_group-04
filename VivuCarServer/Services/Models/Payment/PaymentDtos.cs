using System.ComponentModel.DataAnnotations;

namespace Services.Models.Payment;

public class CreatePaymentRequest
{
    public int BookingId { get; set; }

    [Required]
    public string Method { get; set; } = string.Empty; // vnpay, momo, cash

    [Required]
    public string ReturnUrl { get; set; } = string.Empty; // URL to redirect customer back to client app
}

public class CreatePaymentResponse
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PaymentCallbackQuery
{
    [Required]
    public string TransactionCode { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty; // success, failed
}

public class PaymentStatusResponse
{
    public int BookingId { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
}
