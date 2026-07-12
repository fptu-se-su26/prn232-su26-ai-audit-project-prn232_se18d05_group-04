using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Owner;

namespace API.Controllers;

[ApiController]
[Route("api/owner")]
[Authorize]
public class OwnerIncidentsController(IIncidentService incidentService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(value, out var id)) return id;
        throw new UnauthorizedAccessException("User claims are invalid or missing.");
    }

    [HttpPost("bookings/{bookingId:int}/incidents")]
    public async Task<IActionResult> CreateIncident(int bookingId, [FromBody] CreateIncidentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await incidentService.CreateIncidentAsync(ownerId, bookingId, request, cancellationToken);
            return CreatedAtAction(nameof(GetIncidentDetail), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("incidents")]
    public async Task<IActionResult> GetOwnerIncidents(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var ownerId = GetCurrentUserId();
        var filter = new OwnerIncidentFilter { Status = status, Page = page, PageSize = pageSize };
        var result = await incidentService.GetOwnerIncidentsAsync(ownerId, filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("incidents/{id:int}")]
    public async Task<IActionResult> GetIncidentDetail(int id, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await incidentService.GetIncidentDetailAsync(ownerId, id, cancellationToken);
        if (result == null) return NotFound(new { message = "Không tìm thấy sự cố." });
        return Ok(result);
    }
}
