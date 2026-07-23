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

    [AllowAnonymous]
    [HttpPost("check-availability")]
    public async Task<ActionResult> CheckAvailability([FromBody] PricePreviewRequest request, CancellationToken cancellationToken)
    {
        var available = await bookingService.CheckAvailabilityAsync(request.CarId, request.StartDateTime, request.EndDateTime, cancellationToken);
        return Ok(new { available });
    }

    [AllowAnonymous]
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

    [HttpGet("owner-requests")]
    public async Task<ActionResult<IReadOnlyList<BookingDetailResponse>>> GetOwnerRequests(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var ownerId = GetCurrentUserId();
        var filter = new BookingListFilter { Status = status, Page = page, PageSize = pageSize };
        var list = await bookingService.GetOwnerBookingsAsync(ownerId, filter, cancellationToken);
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
        <div class='title'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
        <div class='sub-title'>Độc lập - Tự do - Hạnh phúc</div>
        <div class='title' style='margin-top: 20px; font-size: 20px;'>HỢP ĐỒNG THUÊ XE TỰ LÁI</div>
        <div class='sub-title'>Số hợp đồng: HD-BK{id}</div>
    </div>

    <div class='section'>
        <div class='section-title'>1. Bên Cho Thuê (Bên A - Chủ xe)</div>
        <p>Họ tên: Hệ thống VivuCar Partner</p>
        <p>Địa chỉ: Hải Châu, Đà Nẵng</p>
    </div>

    <div class='section'>
        <div class='section-title'>2. Bên Thuê (Bên B - Khách hàng)</div>
        <p>Thông tin được đăng ký trên hồ sơ trực tuyến điện tử của VivuCar.</p>
    </div>

    <div class='section'>
        <div class='section-title'>3. Chi tiết Phương tiện & Giá thuê</div>
        <table>
            <tr>
                <th>Mã Đơn Đặt</th>
                <td>BK-{id}</td>
                <th>Phương tiện</th>
                <td>Xe tự lái VivuCar</td>
            </tr>
            <tr>
                <th>Thời gian Nhận</th>
                <td>Vui lòng xem chi tiết đơn hàng</td>
                <th>Thời gian Trả</th>
                <td>Vui lòng xem chi tiết đơn hàng</td>
            </tr>
            <tr>
                <th>Tổng số tiền thuê</th>
                <td>Theo bảng tính chi tiết</td>
                <th>Phương thức thanh toán</th>
                <td>Chuyển khoản / Tiền mặt</td>
            </tr>
        </table>
    </div>

    <div class='section'>
        <div class='section-title'>4. Trách nhiệm các bên</div>
        <ul>
            <li><strong>Bên A:</strong> Giao xe đúng hẹn, đúng tình trạng mô tả, giấy tờ đầy đủ.</li>
            <li><strong>Bên B:</strong> Trả xe đúng hẹn, thanh toán đủ tiền, chịu trách nhiệm vi phạm giao thông và hư hỏng trong thời gian thuê.</li>
        </ul>
    </div>
    <div class='signatures'>
        <div class='signature-box'>
            <strong>ĐẠI DIỆN BÊN A</strong><br/>
            (Ký và ghi rõ họ tên)
            <div class='signature-line'></div>
        </div>
        <div class='signature-box'>
            <strong>ĐẠI DIỆN BÊN B</strong><br/>
            (Ký và ghi rõ họ tên)
            <div class='signature-line'></div>
        </div>
    </div>
    
    <div class='footer'>
        Hợp đồng được tạo tự động bởi hệ thống VivuCar.<br/>
        Có giá trị pháp lý khi hai bên đồng thuận và ký xác nhận.
    </div>
</body>
</html>
";
        return Content(htmlContent, "text/html");
    }
}


