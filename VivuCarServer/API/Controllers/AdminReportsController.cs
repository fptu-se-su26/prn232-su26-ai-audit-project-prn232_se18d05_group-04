using API.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminReportsController(IAdminReportService reportService) : ControllerBase
{
    [HttpGet("revenue")]
    [ProducesResponseType<AdminRevenueReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminRevenueReportResponse>> GetRevenue(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var resolvedTo = to ?? today;
        var resolvedFrom = from ?? new DateOnly(resolvedTo.Year, resolvedTo.Month, 1);

        try
        {
            return Ok(await reportService.GetRevenueAsync(resolvedFrom, resolvedTo, cancellationToken));
        }
        catch (AdminReportValidationException exception)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, exception.Message);
        }
    }
}
