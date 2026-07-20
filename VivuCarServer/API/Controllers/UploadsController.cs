using API.Models;
using API.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize] // Any authenticated user can upload
public class UploadsController(IFileStorageService fileStorageService) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<StoredFileResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoredFileResult>> Upload(
        IFormFile file,
        string? folder,
        CancellationToken cancellationToken
    )
    {
        if (file == null || file.Length == 0)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "No file provided.");
        }

        try
        {
            // E.g. folder="documents" or "avatars"
            var targetFolder = string.IsNullOrWhiteSpace(folder) ? "general" : folder;

            var storedFile = await fileStorageService.SaveAsync(file, targetFolder, cancellationToken);
            return Ok(storedFile);
        }
        catch (InvalidFileUploadException exception)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status500InternalServerError, $"Upload failed: {exception.Message}");
        }
    }
}
