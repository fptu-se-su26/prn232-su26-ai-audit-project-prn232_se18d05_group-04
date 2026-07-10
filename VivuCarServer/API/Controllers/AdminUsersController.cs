using System.IdentityModel.Tokens.Jwt;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminUsersController(IAdminUserService adminUserService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminUserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminUserResponse>>> GetUsers(
        CancellationToken cancellationToken
    )
    {
        return Ok(await adminUserService.GetUsersAsync(cancellationToken));
    }

    [HttpPatch("{userId:int}/lock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Lock(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var adminIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(adminIdValue, out var adminId))
        {
            return Unauthorized();
        }

        if (userId == adminId)
        {
            return BadRequest(new { message = "An admin cannot lock their own account." });
        }

        var succeeded = await adminUserService.LockAsync(
            userId,
            adminId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken
        );

        return succeeded ? NoContent() : NotFound();
    }

    [HttpPatch("{userId:int}/unlock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unlock(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var succeeded = await adminUserService.UnlockAsync(
            userId,
            cancellationToken
        );

        return succeeded ? NoContent() : NotFound();
    }
}
