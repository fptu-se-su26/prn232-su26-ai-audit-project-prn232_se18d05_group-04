using API.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/moderation")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminModerationController(IAdminModerationService service) : ControllerBase
{
    [HttpGet("content")]
    public async Task<ActionResult<IReadOnlyList<AdminModerationContentResponse>>> GetContent(CancellationToken cancellationToken)
        => Ok(await service.GetContentAsync(cancellationToken));

    [HttpGet("licenses")]
    public async Task<ActionResult<IReadOnlyList<AdminLicenseModerationResponse>>> GetLicenses(CancellationToken cancellationToken)
        => Ok(await service.GetLicensesAsync(cancellationToken));

    [HttpPost("licenses/{id:int}/ocr")]
    public async Task<ActionResult<AdminLicenseOcrResponse>> ScanLicense(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.ScanLicenseAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DriverLicenseOcrException exception)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status422UnprocessableEntity, exception.Message);
        }
    }

    [HttpPatch("licenses/{id:int}/approve")]
    public Task<IActionResult> ApproveLicense(int id, CancellationToken cancellationToken)
        => SetLicenseStatus(id, "approved", cancellationToken);

    [HttpPatch("licenses/{id:int}/reject")]
    public Task<IActionResult> RejectLicense(int id, CancellationToken cancellationToken)
        => SetLicenseStatus(id, "rejected", cancellationToken);

    private async Task<IActionResult> SetLicenseStatus(int id, string status, CancellationToken cancellationToken)
        => await service.SetLicenseStatusAsync(id, status, cancellationToken) ? NoContent() : NotFound();
}
