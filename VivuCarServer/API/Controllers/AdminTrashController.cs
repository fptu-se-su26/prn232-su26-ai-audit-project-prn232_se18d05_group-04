using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/trash")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminTrashController(IAdminUserService adminUserService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminTrashItemResponse>>(
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IReadOnlyList<AdminTrashItemResponse>>> Get(
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await adminUserService.GetDeletedCustomersAsync(cancellationToken)
        );
    }

    [HttpPatch("users/{userId:int}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreCustomer(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var succeeded = await adminUserService.RestoreCustomerAsync(
            userId,
            cancellationToken
        );

        return succeeded
            ? NoContent()
            : NotFound(new { message = "Deleted customer account was not found." });
    }
}