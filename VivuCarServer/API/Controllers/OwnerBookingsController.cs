using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Owner;

namespace API.Controllers;

[ApiController]
[Route("api/owner")]
[Authorize]
public class OwnerBookingsController(IOwnerBookingService ownerBookingService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(value, out var id)) return id;
        throw new UnauthorizedAccessException("User claims are invalid or missing.");
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await ownerBookingService.GetOwnerDashboardStatsAsync(ownerId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetOwnerBookings(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var ownerId = GetCurrentUserId();
        var filter = new OwnerBookingListFilter { Status = status, Page = page, PageSize = pageSize };
        var result = await ownerBookingService.GetOwnerBookingsAsync(ownerId, filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings/{id:int}")]
    public async Task<IActionResult> GetOwnerBookingDetail(int id, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await ownerBookingService.GetOwnerBookingDetailAsync(ownerId, id, cancellationToken);
        if (result == null) return NotFound(new { message = "Không tìm thấy chuyến thuê." });
        return Ok(result);
    }

    [HttpPost("bookings/{id:int}/handover")]
    public async Task<IActionResult> ConfirmHandover(int id, [FromBody] HandoverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerBookingService.ConfirmHandoverAsync(ownerId, id, request, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy chuyến thuê." });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("bookings/{id:int}/return")]
    public async Task<IActionResult> ConfirmReturn(int id, [FromBody] ReturnInspectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerBookingService.ConfirmReturnAsync(ownerId, id, request, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy chuyến thuê." });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("bookings/{id:int}/complete")]
    public async Task<IActionResult> CompleteBooking(int id, [FromBody] CompleteBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerBookingService.CompleteBookingAsync(ownerId, id, request.NextCarStatus, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy chuyến thuê." });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("bookings/{id:int}/approve")]
    public async Task<IActionResult> ApproveBooking(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerBookingService.ApproveBookingAsync(ownerId, id, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy đơn đặt xe." });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("bookings/{id:int}/reject")]
    public async Task<IActionResult> RejectBooking(int id, [FromBody] RejectBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerBookingService.RejectBookingAsync(ownerId, id, request.Reason, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy đơn đặt xe." });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}

public class CompleteBookingRequest
{
    public string NextCarStatus { get; set; } = "available";
}

public class RejectBookingRequest
{
    public string? Reason { get; set; }
}
