using System.IdentityModel.Tokens.Jwt;
using API.Models;
using API.Services.Storage;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/cars")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminCarsController(
    IAdminCarService adminCarService,
    IFileStorageService fileStorageService
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<AdminCarResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminCarResponse>>> GetCars(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? keyword,
        [FromQuery] string? status,
        [FromQuery(Name = "type_id")] int? typeId,
        [FromQuery(Name = "fuel_type")] string? fuelType,
        [FromQuery] string? transmission,
        CancellationToken cancellationToken
    )
    {
        var query = new AdminCarListQuery
        {
            Page = page <= 0 ? 1 : page,
            PageSize = pageSize <= 0 ? 10 : pageSize,
            Keyword = keyword,
            Status = status,
            TypeId = typeId,
            FuelType = fuelType,
            Transmission = transmission
        };

        return Ok(await adminCarService.GetCarsAsync(query, cancellationToken));
    }

    [HttpGet("{carId:int}")]
    [ProducesResponseType<AdminCarResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminCarResponse>> GetCar(int carId, CancellationToken cancellationToken)
    {
        var car = await adminCarService.GetCarAsync(carId, cancellationToken);
        return car is null
            ? ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.")
            : Ok(car);
    }

    [HttpPost]
    [ProducesResponseType<AdminCarResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminCarResponse>> CreateCar(AdminCarUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        try
        {
            var created = await adminCarService.CreateCarAsync(adminUserId, request, cancellationToken);
            return CreatedAtAction(nameof(GetCar), new { carId = created.Id }, created);
        }
        catch (AdminCarServiceException exception)
        {
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    [HttpPut("{carId:int}")]
    [ProducesResponseType<AdminCarResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminCarResponse>> UpdateCar(int carId, AdminCarUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        try
        {
            var updated = await adminCarService.UpdateCarAsync(adminUserId, carId, request, cancellationToken);
            return updated is null
                ? ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.")
                : Ok(updated);
        }
        catch (AdminCarServiceException exception)
        {
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    [HttpPatch("{carId:int}/block")]
    [ProducesResponseType<AdminCarResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminCarResponse>> BlockCar(int carId, AdminCarBlockRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        try
        {
            var updated = await adminCarService.BlockCarAsync(adminUserId, carId, request, cancellationToken);
            return updated is null
                ? ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.")
                : Ok(updated);
        }
        catch (AdminCarServiceException exception)
        {
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    [HttpPatch("{carId:int}/unblock")]
    [ProducesResponseType<AdminCarResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminCarResponse>> UnblockCar(int carId, AdminCarUnblockRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        try
        {
            var updated = await adminCarService.UnblockCarAsync(adminUserId, carId, request, cancellationToken);
            return updated is null
                ? ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.")
                : Ok(updated);
        }
        catch (AdminCarServiceException exception)
        {
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    [HttpPost("{carId:int}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<IReadOnlyList<AdminCarImageResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AdminCarImageResponse>>> AddImages(
        int carId,
        [FromForm] List<IFormFile> files,
        [FromForm] bool isPrimary,
        CancellationToken cancellationToken
    )
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        if (files.Count == 0)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "Validation failed.", new Dictionary<string, string[]>
            {
                ["files"] = ["At least one image is required."]
            });
        }

        var storedFiles = new List<StoredFileResult>();
        try
        {
            foreach (var file in files)
            {
                storedFiles.Add(await fileStorageService.SaveAsync(file, $"uploads/cars/{carId}", cancellationToken));
            }

            var imageRequests = storedFiles.Select((file, index) => new AdminCarImageCreateRequest
            {
                ImageUrl = file.PublicUrl,
                IsPrimary = isPrimary && index == 0
            }).ToList();

            var images = await adminCarService.AddImagesAsync(adminUserId, carId, imageRequests, cancellationToken);
            if (images is null)
            {
                await DeleteStoredFilesAsync(storedFiles, cancellationToken);
                return ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.");
            }

            return StatusCode(StatusCodes.Status201Created, images);
        }
        catch (InvalidFileUploadException exception)
        {
            await DeleteStoredFilesAsync(storedFiles, cancellationToken);
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "Validation failed.", new Dictionary<string, string[]>
            {
                ["files"] = [exception.Message]
            });
        }
        catch (AdminCarServiceException exception)
        {
            await DeleteStoredFilesAsync(storedFiles, cancellationToken);
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    [HttpDelete("{carId:int}/images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(int carId, int imageId, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        var deletedImage = await adminCarService.DeleteImageAsync(adminUserId, carId, imageId, cancellationToken);
        if (deletedImage is null)
        {
            return ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Image was not found.");
        }

        await fileStorageService.DeleteAsync(deletedImage.ImageUrl, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{carId:int}/images/{imageId:int}/primary")]
    [ProducesResponseType<AdminCarImageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminCarImageResponse>> SetPrimaryImage(int carId, int imageId, CancellationToken cancellationToken)
    {
        if (!TryGetAdminUserId(out var adminUserId)) return UnauthorizedError();

        try
        {
            var image = await adminCarService.SetPrimaryImageAsync(adminUserId, carId, imageId, cancellationToken);
            return image is null
                ? ApiErrorFactory.Error(HttpContext, StatusCodes.Status404NotFound, "Car was not found.")
                : Ok(image);
        }
        catch (AdminCarServiceException exception)
        {
            return ApiErrorFactory.FromServiceException(HttpContext, exception);
        }
    }

    private bool TryGetAdminUserId(out int adminUserId)
    {
        var adminIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(adminIdValue, out adminUserId);
    }

    private ObjectResult UnauthorizedError()
    {
        return ApiErrorFactory.Error(HttpContext, StatusCodes.Status401Unauthorized, "Authentication is required.");
    }

    private async Task DeleteStoredFilesAsync(IEnumerable<StoredFileResult> storedFiles, CancellationToken cancellationToken)
    {
        foreach (var storedFile in storedFiles)
        {
            await fileStorageService.DeleteAsync(storedFile.PublicUrl, cancellationToken);
        }
    }
}

[ApiController]
[Route("api/car-types")]
[Authorize]
public class CarTypesController(IAdminCarService adminCarService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CarTypeResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CarTypeResponse>>> GetCarTypes(CancellationToken cancellationToken)
    {
        return Ok(await adminCarService.GetCarTypesAsync(cancellationToken));
    }
}

[ApiController]
[Route("api/car-models")]
[Authorize]
public class CarModelsController(IAdminCarService adminCarService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CarModelResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CarModelResponse>>> GetCarModels(CancellationToken cancellationToken)
    {
        return Ok(await adminCarService.GetCarModelsAsync(cancellationToken));
    }
}
