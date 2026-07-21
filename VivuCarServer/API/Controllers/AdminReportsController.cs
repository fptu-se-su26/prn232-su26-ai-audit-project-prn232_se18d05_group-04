using Repositories.Models;
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
    [HttpGet("preview")]
    [ProducesResponseType<AdminReportPreviewResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminReportPreviewResponse>> Preview(
        [FromServices] IAdminExportService exportService,
        [FromQuery] string type,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] string? paymentStatus,
        [FromQuery] string? bookingStatus,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var filter = new AdminReportFilter { Type = type, From = from ?? new DateOnly(today.Year,today.Month,1), To = to ?? today, PaymentStatus = paymentStatus, BookingStatus = bookingStatus };
        try { return Ok(await exportService.PreviewAsync(filter,cancellationToken)); }
        catch (AdminExportValidationException exception) { return ApiErrorFactory.Error(HttpContext,StatusCodes.Status400BadRequest,exception.Message); }
    }
    [HttpGet("revenue")]
    [ProducesResponseType<AdminRevenueReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminRevenueReportResponse>> GetRevenue(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var resolvedTo = to ?? today;
        var resolvedFrom = from ?? new DateOnly(resolvedTo.Year, resolvedTo.Month, 1);

        try
        {
            return Ok(await reportService.GetRevenueAsync(
                resolvedFrom,
                resolvedTo,
                page ?? 1,
                pageSize ?? 5,
                cancellationToken));
        }
        catch (AdminReportValidationException exception)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, exception.Message);
        }
    }
}
