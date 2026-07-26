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

    [HttpPost("{id:int}/request-return")]
    public async Task<ActionResult<BookingDetailResponse>> RequestReturn(int id, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var booking = await bookingService.RequestReturnAsync(customerId, id, cancellationToken);
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
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
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
    public async Task<IActionResult> GetContractPdf(int id, [FromQuery] string? sig, CancellationToken cancellationToken)
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
    <title>Hợp đồng thuê xe tự lái VivuCar</title>
    <style>
        body {{ font-family: 'Times New Roman', Times, serif; margin: 40px; color: #111; line-height: 1.5; font-size: 15px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .title {{ font-size: 22px; font-weight: bold; text-transform: uppercase; margin: 0; }}
        .sub-title {{ font-size: 16px; font-weight: bold; text-decoration: underline; margin-top: 5px; }}
        .contract-title {{ font-size: 24px; font-weight: bold; text-transform: uppercase; margin-top: 30px; text-align: center; }}
        .contract-no {{ text-align: center; font-style: italic; margin-bottom: 30px; }}
        .section {{ margin-bottom: 20px; }}
        .section-title {{ font-weight: bold; text-transform: uppercase; margin-bottom: 10px; font-size: 16px; }}
        .info-row {{ margin-bottom: 5px; display: flex; }}
        .info-label {{ font-weight: bold; width: 200px; flex-shrink: 0; }}
        .info-value {{ flex-grow: 1; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 15px; margin-bottom: 20px; }}
        th, td {{ border: 1px solid #333; padding: 10px; text-align: left; }}
        th {{ background-color: #f0f0f0; font-weight: bold; text-align: center; }}
        .signatures {{ margin-top: 50px; display: flex; justify-content: space-between; page-break-inside: avoid; }}
        .signature-box {{ text-align: center; width: 45%; }}
        .signature-line {{ margin-top: 80px; border-top: 1px dashed #333; width: 200px; margin-left: auto; margin-right: auto; }}
        .footer {{ text-align: center; margin-top: 60px; font-size: 13px; font-style: italic; }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='title'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
        <div class='sub-title'>Độc lập - Tự do - Hạnh phúc</div>
    </div>
    
    <div class='contract-title'>HỢP ĐỒNG THUÊ XE TỰ LÁI</div>
    <div class='contract-no'>Số: HD-{contract.BookingCode}</div>

    <div style='margin-bottom: 20px; font-style: italic;'>
        Hôm nay, ngày {DateTime.UtcNow.Day:00} tháng {DateTime.UtcNow.Month:00} năm {DateTime.UtcNow.Year}, tại hệ thống VivuCar, chúng tôi gồm có:
    </div>

    <div class='section'>
        <div class='section-title'>ĐIỀU 1: ĐẠI DIỆN BÊN CHO THUÊ (BÊN A)</div>
        <div class='info-row'><span class='info-label'>Hệ thống:</span> <span class='info-value'>VivuCar Partner</span></div>
        <div class='info-row'><span class='info-label'>Địa chỉ:</span> <span class='info-value'>Khu vực Đà Nẵng</span></div>
        <div class='info-row'><span class='info-label'>Điện thoại hỗ trợ:</span> <span class='info-value'>1900 9999</span></div>
    </div>

    <div class='section'>
        <div class='section-title'>ĐIỀU 2: ĐẠI DIỆN BÊN THUÊ (BÊN B)</div>
        <div class='info-row'><span class='info-label'>Ông/Bà:</span> <span class='info-value'>{contract.DriverInfo?.FullName ?? "Khách hàng"}</span></div>
        <div class='info-row'><span class='info-label'>Số điện thoại:</span> <span class='info-value'>{contract.DriverInfo?.PhoneNumber ?? "Không có"}</span></div>
    </div>

    <div class='section'>
        <div class='section-title'>ĐIỀU 3: THÔNG TIN PHƯƠNG TIỆN THUÊ</div>
        <div class='info-row'><span class='info-label'>Loại xe:</span> <span class='info-value'>{contract.CarName}</span></div>
        <div class='info-row'><span class='info-label'>Biển số xe:</span> <span class='info-value'>{contract.LicensePlate}</span></div>
        <div class='info-row'><span class='info-label'>Màu sắc:</span> <span class='info-value'>Được cập nhật trên hệ thống</span></div>
        <div class='info-row'><span class='info-label'>Số chỗ ngồi:</span> <span class='info-value'>Được cập nhật trên hệ thống</span></div>
    </div>

    <div class='section'>
        <div class='section-title'>ĐIỀU 4: THỜI GIAN VÀ CHI PHÍ THUÊ XE</div>
        <table>
            <tr>
                <th>Thời gian Nhận xe</th>
                <th>Thời gian Trả xe</th>
                <th>Tổng số ngày thuê</th>
            </tr>
            <tr>
                <td style='text-align: center;'>{contract.StartDateTime:dd/MM/yyyy HH:mm}</td>
                <td style='text-align: center;'>{contract.EndDateTime:dd/MM/yyyy HH:mm}</td>
                <td style='text-align: center;'>{Math.Ceiling((contract.EndDateTime - contract.StartDateTime).TotalDays)} ngày</td>
            </tr>
        </table>
        
        <div class='info-row'><span class='info-label'>Tổng tiền thuê:</span> <span class='info-value' style='font-weight: bold;'>{contract.TotalAmount:N0} VNĐ</span></div>
        <div class='info-row'><span class='info-label'>Số tiền đã cọc:</span> <span class='info-value'>{contract.DepositAmount:N0} VNĐ</span></div>
        <div class='info-row'><span class='info-label'>Số tiền còn lại:</span> <span class='info-value'>{contract.RemainingAmount:N0} VNĐ (Thanh toán khi trả xe)</span></div>
        <div class='info-row'><span class='info-label'>Phương thức:</span> <span class='info-value'>Chuyển khoản / Tiền mặt</span></div>
    </div>

    <div class='section'>
        <div class='section-title'>ĐIỀU 5: TRÁCH NHIỆM VÀ CAM KẾT</div>
        <ul style='padding-left: 20px; text-align: justify;'>
            <li style='margin-bottom: 8px;'><strong>Trách nhiệm Bên A:</strong> Giao xe đúng hạn, đúng loại xe, biển số, đảm bảo xe hoạt động tốt, đầy đủ giấy tờ xe hợp lệ (bản photo công chứng hoặc bản gốc tùy thỏa thuận).</li>
            <li style='margin-bottom: 8px;'><strong>Trách nhiệm Bên B:</strong> Sử dụng xe đúng mục đích, không sử dụng xe vào mục đích cầm cố, thế chấp hay vận chuyển hàng cấm. Chịu hoàn toàn trách nhiệm dân sự và hình sự trước pháp luật về mọi hành vi vi phạm pháp luật trong thời gian thuê xe.</li>
            <li style='margin-bottom: 8px;'><strong>Bảo quản tài sản:</strong> Bên B phải bồi thường 100% chi phí sửa chữa nếu xảy ra va quệt, hư hỏng do lỗi của Bên B. Nếu xe bị giam giữ do vi phạm luật giao thông, Bên B phải chịu mọi chi phí phạt và bồi thường tiền thuê xe trong những ngày xe bị giam.</li>
        </ul>
    </div>
    
    <div style='margin-top: 30px;'>Hai bên đã đọc, hiểu rõ và đồng ý với các điều khoản trên. Hợp đồng có hiệu lực kể từ thời điểm ký xác nhận điện tử.</div>

    <div class='signatures'>
        <div class='signature-box'>
            <strong>ĐẠI DIỆN BÊN A</strong><br/>
            (Ký xác nhận hệ thống)
            <div style='margin-top:20px; font-style: italic; color: #0056b3; font-weight: bold; border: 2px solid #0056b3; display: inline-block; padding: 10px 20px; transform: rotate(-5deg);'>
                ĐÃ DUYỆT<br/>VivuCar System
            </div>
        </div>
        <div class='signature-box'>
            <strong>ĐẠI DIỆN BÊN B</strong><br/>
            (Khách hàng ký và ghi rõ họ tên)
            {(string.IsNullOrEmpty(sig) ? "<div class='signature-line'></div>" : $"<div style='margin-top:20px;'><img src='{sig}' style='max-width:250px; max-height:120px; mix-blend-mode: multiply;'/></div>")}
        </div>
    </div>
    
    <div class='footer'>
        Hợp đồng được tạo tự động và lưu trữ trên hệ thống VivuCar.<br/>
        VivuCar.vn © {DateTime.UtcNow.Year}
    </div>
</body>
</html>
";
        return Content(htmlContent, "text/html");
    }

    public class SignContractRequest
    {
        public string SignatureUrl { get; set; } = string.Empty;
    }

    [HttpPost("{id:int}/contract/sign")]
    public async Task<IActionResult> SignContract(int id, [FromBody] SignContractRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        var contract = await bookingService.GetContractAsync(customerId, id, cancellationToken);
        if (contract == null) return NotFound(new { message = "Booking contract not found or access denied." });

        if (string.IsNullOrWhiteSpace(request.SignatureUrl))
        {
            return BadRequest(new { message = "SignatureUrl is required." });
        }

        var success = await bookingService.UpdateContractSignatureAsync(customerId, id, request.SignatureUrl, cancellationToken);
        if (!success)
        {
            return BadRequest(new { message = "Failed to update contract signature. Make sure the contract exists and you have permission." });
        }

        return Ok(new { message = "Signature saved successfully.", signatureUrl = request.SignatureUrl });
    }
}


