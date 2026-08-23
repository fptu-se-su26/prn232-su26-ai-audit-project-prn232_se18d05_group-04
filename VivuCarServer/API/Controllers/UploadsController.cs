using API.Models;
using API.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/uploads")]
// [Authorize] // Any authenticated user can upload
public class UploadsController(IFileStorageService fileStorageService) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<StoredFileResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoredFileResult>> Upload(
        IFormFile file,
        [FromQuery] string? folder,
        CancellationToken cancellationToken
    )
    {
        if (file == null)
        {
            Console.WriteLine("UploadsController: file is NULL!");
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "No file provided.");
        }
        if (file.Length == 0)
        {
            Console.WriteLine("UploadsController: file length is 0!");
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "File is empty.");
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
