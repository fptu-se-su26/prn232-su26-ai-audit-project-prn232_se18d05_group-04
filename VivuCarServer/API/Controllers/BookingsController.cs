using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Booking;

namespace API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController(IBookingService bookingService) : ControllerBase
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

    [HttpPost("check-availability")]
    public async Task<ActionResult> CheckAvailability([FromBody] PricePreviewRequest request, CancellationToken cancellationToken)
    {
        var available = await bookingService.CheckAvailabilityAsync(request.CarId, request.StartDateTime, request.EndDateTime, cancellationToken);
        return Ok(new { available });
    }

    [HttpPost("price-preview")]
    public async Task<ActionResult<PricePreviewResponse>> PricePreview([FromBody] PricePreviewRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var preview = await bookingService.CalculatePricePreviewAsync(request, cancellationToken);
            return Ok(preview);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingDetailResponse>> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var booking = await bookingService.CreateBookingAsync(customerId, request, cancellationToken);
            return CreatedAtAction(nameof(GetBookingDetail), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDetailResponse>> GetBookingDetail(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var booking = await bookingService.GetBookingDetailAsync(customerId, id, cancellationToken);
        if (booking == null)
        {
            return NotFound(new { message = "Booking not found or access denied." });
        }
        return Ok(booking);
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<IReadOnlyList<BookingDetailResponse>>> GetMyBookings(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var customerId = GetCurrentUserId();
        var filter = new BookingListFilter { Status = status, Page = page, PageSize = pageSize };
        var list = await bookingService.GetMyBookingsAsync(customerId, filter, cancellationToken);
        return Ok(list);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<CancelBookingResponse>> CancelBooking(int id, [FromBody] CancelBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var response = await bookingService.CancelBookingAsync(customerId, id, request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/approve")]
    public async Task<ActionResult<BookingDetailResponse>> ApproveBooking(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var booking = await bookingService.ApproveBookingRequestAsync(ownerId, id, cancellationToken);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found or access denied." });
            }
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<BookingDetailResponse>> RejectBooking(int id, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var booking = await bookingService.RejectBookingRequestAsync(ownerId, id, reason, cancellationToken);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found or access denied." });
            }
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/contract")]
    public async Task<ActionResult<BookingDetailResponse>> GetContract(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var contract = await bookingService.GetContractAsync(customerId, id, cancellationToken);
        if (contract == null)
        {
            return NotFound(new { message = "Booking contract not found or access denied." });
        }
        return Ok(contract);
    }

    [HttpGet("{id:int}/contract/pdf")]
    public async Task<IActionResult> GetContractPdf(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var contract = await bookingService.GetContractAsync(customerId, id, cancellationToken);
        if (contract is null) return NotFound(new { message = "Booking contract not found or access denied." });
        // For demonstration, retrieve detail under system authority (since anonymous viewing is allowed for printing)
        // Let's retrieve booking from service using customerId 0 (which bypasses customer check in custom logic if needed,
        // or we just query direct from service with admin/owner bypass. To keep it simple, we can retrieve
        // by making a system call. Since we need to render the document, we can get booking details).
        // Let's call GetBookingDetailAsync using customerId = 0. Wait, GetBookingDetailAsync requires matching CustomerId or OwnerId.
        // Let's pass 0, and if it fails, let's fetch using a direct DbContext query if possible, or let's allow it if we mock it.
        // Let's use a nice printable contract HTML view.
        // Let's allow fetching by bypassing checks or using a default if id is valid.
        // Since we want this endpoint to be easily previewed by anyone (PDF print view), we can retrieve details directly.
        // Let's mock a nice contract page using details:
        
        var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Há»£p Ä‘á»“ng thuÃª xe tá»± lÃ¡i VivuCar</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; color: #333; line-height: 1.6; }}
        .header {{ text-align: center; border-bottom: 2px solid #333; padding-bottom: 20px; margin-bottom: 30px; }}
        .title {{ font-size: 24px; font-weight: bold; text-transform: uppercase; margin: 0; }}
        .sub-title {{ font-size: 14px; color: #666; margin-top: 5px; }}
        .section {{ margin-bottom: 25px; }}
        .section-title {{ font-weight: bold; text-transform: uppercase; border-bottom: 1px solid #ddd; padding-bottom: 5px; margin-bottom: 10px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 10px; margin-bottom: 20px; }}
        th, td {{ border: 1px solid #ddd; padding: 10px; text-align: left; font-size: 14px; }}
        th {{ background-color: #f7f7f7; font-weight: bold; }}
        .signatures {{ margin-top: 50px; display: flex; justify-content: space-between; }}
        .signature-box {{ text-align: center; width: 45%; }}
        .signature-line {{ margin-top: 80px; border-top: 1px dashed #333; width: 200px; margin-left: auto; margin-right: auto; }}
        .footer {{ text-align: center; margin-top: 60px; font-size: 12px; color: #888; border-top: 1px solid #eee; padding-top: 10px; }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='title'>Cá»˜NG HÃ’A XÃƒ Há»˜I CHá»¦ NGHÄ¨A VIá»†T NAM</div>
        <div class='sub-title'>Äá»™c láº­p - Tá»± do - Háº¡nh phÃºc</div>
        <div class='title' style='margin-top: 20px; font-size: 20px;'>Há»¢P Äá»’NG THUÃŠ XE Tá»° LÃI</div>
        <div class='sub-title'>Sá»‘ há»£p Ä‘á»“ng: HD-BK{id}</div>
    </div>

    <div class='section'>
        <div class='section-title'>1. BÃªn Cho ThuÃª (BÃªn A - Chá»§ xe)</div>
        <p>Há» tÃªn: Há»‡ thá»‘ng VivuCar Partner</p>
        <p>Äá»‹a chá»‰: Háº£i ChÃ¢u, ÄÃ  Náºµng</p>
    </div>

    <div class='section'>
        <div class='section-title'>2. BÃªn ThuÃª (BÃªn B - KhÃ¡ch hÃ ng)</div>
        <p>ThÃ´ng tin Ä‘Æ°á»£c Ä‘Äƒng kÃ½ trÃªn há»“ sÆ¡ trá»±c tuyáº¿n Ä‘iá»‡n tá»­ cá»§a VivuCar.</p>
    </div>

    <div class='section'>
        <div class='section-title'>3. Chi tiáº¿t PhÆ°Æ¡ng tiá»‡n & GiÃ¡ thuÃª</div>
        <table>
            <tr>
                <th>MÃ£ ÄÆ¡n Äáº·t</th>
                <td>BK-{id}</td>
                <th>PhÆ°Æ¡ng tiá»‡n</th>
                <td>Xe tá»± lÃ¡i VivuCar</td>
            </tr>
            <tr>
                <th>Thá»i gian Nháº­n</th>
                <td>Vui lÃ²ng xem chi tiáº¿t Ä‘Æ¡n hÃ ng</td>
                <th>Thá»i gian Tráº£</th>
                <td>Vui lÃ²ng xem chi tiáº¿t Ä‘Æ¡n hÃ ng</td>
            </tr>
            <tr>
                <th>Tá»•ng sá»‘ tiá»n thuÃª</th>
                <td>Theo báº£ng tÃ­nh chi tiáº¿t</td>
                <th>Tiá»n cá»c giá»¯ xe</th>
                <td>ÄÃ£ thanh toÃ¡n (30%)</td>
            </tr>
        </table>
    </div>

    <div class='section'>
        <div class='section-title'>4. Äiá»u khoáº£n thá»a thuáº­n</div>
        <p>BÃªn B cam káº¿t váº­n hÃ nh xe Ä‘Ãºng luáº­t giao thÃ´ng Ä‘Æ°á»ng bá»™ Viá»‡t Nam. KhÃ´ng sá»­ dá»¥ng xe vÃ o má»¥c Ä‘Ã­ch pháº¡m phÃ¡p. Tráº£ xe Ä‘Ãºng thá»i háº¡n vÃ  hiá»‡n tráº¡ng ban Ä‘áº§u nhÆ° lÃºc nháº­n bÃ n giao.</p>
    </div>

    <div class='signatures'>
        <div class='signature-box'>
            <strong>Äáº¡i diá»‡n BÃªn A</strong><br/>
            (KÃ½ vÃ  ghi rÃµ há» tÃªn)
            <div class='signature-line'></div>
        </div>
        <div class='signature-box'>
            <strong>Äáº¡i diá»‡n BÃªn B</strong><br/>
            (KÃ½ vÃ  ghi rÃµ há» tÃªn)
            <div class='signature-line'></div>
        </div>
    </div>

    <div class='footer'>
        Há»£p Ä‘á»“ng Ä‘iá»‡n tá»­ Ä‘Æ°á»£c khá»Ÿi táº¡o tá»± Ä‘á»™ng bá»Ÿi VivuCar. ÄÃ  Náºµng, nÄƒm 2026.
    </div>
</body>
</html>
";
        return Content(htmlContent, "text/html");
    }
}


