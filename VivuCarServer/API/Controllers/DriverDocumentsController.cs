using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.User;

namespace API.Controllers;

[ApiController]
[Route("api/driver-documents")]
[Authorize]
public class DriverDocumentsController(IUserService userService) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<DriverDocumentDto>> GetMyDocument(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var document = await userService.GetDriverDocumentAsync(userId.Value, cancellationToken);
        return Ok(document);
    }

    [HttpPost("my/submit")]
    public async Task<ActionResult<DriverDocumentDto>> SubmitDocument([FromBody] SubmitDocumentRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var document = await userService.SubmitDocumentForVerificationAsync(userId.Value, request, cancellationToken);
        return Ok(document);
    }

    [HttpPost("my/cancel")]
    public async Task<ActionResult<DriverDocumentDto>> CancelDocument(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var document = await userService.CancelDocumentVerificationAsync(userId.Value, cancellationToken);
        return Ok(document);
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(idClaim, out var id))
            return id;
        return null;
    }
}
