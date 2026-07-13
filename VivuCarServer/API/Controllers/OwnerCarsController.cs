using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Owner;

namespace API.Controllers;

[ApiController]
[Route("api/owner/cars")]
[Authorize]
public class OwnerCarsController(IOwnerCarService ownerCarService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (int.TryParse(value, out var id)) return id;
        throw new UnauthorizedAccessException("User claims are invalid or missing.");
    }

    [HttpGet]
    public async Task<IActionResult> GetOwnerCars(
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var ownerId = GetCurrentUserId();
        var query = new OwnerCarListQuery { Status = status, Search = search, Page = page, PageSize = pageSize };
        var result = await ownerCarService.GetOwnerCarsAsync(ownerId, query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOwnerCarDetail(int id, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await ownerCarService.GetOwnerCarDetailAsync(ownerId, id, cancellationToken);
        if (result == null) return NotFound(new { message = "Không tìm thấy xe hoặc bạn không có quyền truy cập." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOwnerCar([FromBody] OwnerCarUpsertRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerCarService.CreateOwnerCarAsync(ownerId, request, cancellationToken);
            return CreatedAtAction(nameof(GetOwnerCarDetail), new { id = result.Id }, result);
        }
        catch (OwnerCarServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOwnerCar(int id, [FromBody] OwnerCarUpsertRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerCarService.UpdateOwnerCarAsync(ownerId, id, request, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy xe hoặc bạn không có quyền truy cập." });
            return Ok(result);
        }
        catch (OwnerCarServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateCarStatus(int id, [FromBody] UpdateCarStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerCarService.UpdateCarStatusAsync(ownerId, id, request.Status, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy xe hoặc bạn không có quyền truy cập." });
            return Ok(result);
        }
        catch (OwnerCarServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/images")]
    public async Task<IActionResult> AddCarImages(int id, [FromBody] IReadOnlyList<OwnerCarImageCreateRequest> images, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerCarService.AddCarImagesAsync(ownerId, id, images, cancellationToken);
            if (result == null) return NotFound(new { message = "Không tìm thấy xe hoặc bạn không có quyền truy cập." });
            return Ok(result);
        }
        catch (OwnerCarServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteCarImage(int id, int imageId, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await ownerCarService.DeleteCarImageAsync(ownerId, id, imageId, cancellationToken);
        if (!result) return NotFound(new { message = "Không tìm thấy ảnh." });
        return NoContent();
    }

    [HttpPatch("{id:int}/images/{imageId:int}/primary")]
    public async Task<IActionResult> SetPrimaryImage(int id, int imageId, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId();
            var result = await ownerCarService.SetPrimaryImageAsync(ownerId, id, imageId, cancellationToken);
            if (!result) return NotFound(new { message = "Không tìm thấy xe hoặc ảnh." });
            return Ok(new { message = "Đã đặt ảnh đại diện thành công." });
        }
        catch (OwnerCarServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/activity")]
    public async Task<IActionResult> GetCarActivity(int id, CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var result = await ownerCarService.GetCarActivityAsync(ownerId, id, cancellationToken);
        if (result == null) return NotFound(new { message = "Không tìm thấy xe hoặc bạn không có quyền truy cập." });
        return Ok(result);
    }

    [HttpGet("meta/types")]
    public async Task<IActionResult> GetCarTypes(CancellationToken cancellationToken)
    {
        var result = await ownerCarService.GetCarTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("meta/models")]
    public async Task<IActionResult> GetCarModels(CancellationToken cancellationToken)
    {
        var result = await ownerCarService.GetCarModelsAsync(cancellationToken);
        return Ok(result);
    }
}

public class UpdateCarStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
