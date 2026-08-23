using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Payment;

namespace API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(userIdValue, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User claims are invalid or missing.");
    }

    [HttpPost("deposit/create")]
    public async Task<ActionResult<CreatePaymentResponse>> CreateDepositPayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await paymentService.CreateDepositPaymentAsync(customerId, baseUrl, request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("final/create")]
    public async Task<ActionResult<CreatePaymentResponse>> CreateFinalPayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await paymentService.CreateFinalPaymentAsync(customerId, baseUrl, request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymentCallback(
        [FromQuery] string TransactionCode,
        [FromQuery] string Status,
        [FromQuery] string RedirectUrl,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var query = new PaymentCallbackQuery { TransactionCode = TransactionCode, Status = Status };
            var result = await paymentService.ProcessCallbackAsync(query, cancellationToken);

            // Redirect back to the client-side return URL with transaction status details
            var redirectUrlWithParams = $"{RedirectUrl}?transactionCode={TransactionCode}&status={Status}&bookingId={result.BookingId}";
            return Redirect(redirectUrlWithParams);
        }
        catch (Exception ex)
        {
            // If anything fails, redirect with failed status if redirect url is provided
            if (!string.IsNullOrWhiteSpace(RedirectUrl))
            {
                var errorRedirect = $"{RedirectUrl}?status=failed&error={Uri.EscapeDataString(ex.Message)}";
                return Redirect(errorRedirect);
            }
            return BadRequest(new { message = "Payment callback processing failed.", error = ex.Message });
        }
    }

    [HttpGet("status/{bookingId:int}")]
    public async Task<ActionResult<PaymentStatusResponse>> GetPaymentStatus(int bookingId, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var status = await paymentService.GetPaymentStatusByBookingIdAsync(customerId, bookingId, cancellationToken);
            if (status == null)
            {
                return NotFound(new { message = "Booking or payment not found." });
            }
            return Ok(status);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
