using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.User;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService, API.Services.Storage.IFileStorageService fileStorageService) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var profile = await userService.GetUserProfileAsync(userId.Value, cancellationToken);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var profile = await userService.UpdateProfileAsync(userId.Value, request, cancellationToken);
        return Ok(profile);
    }

    [HttpPost("profile/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var storedFile = await fileStorageService.SaveAsync(file, "avatars", cancellationToken);
        var avatarUrl = await userService.UpdateAvatarUrlAsync(userId.Value, storedFile.PublicUrl, cancellationToken);
        
        return Ok(new { AvatarUrl = avatarUrl });
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(idClaim, out var id))
            return id;
        return null;
    }
}
