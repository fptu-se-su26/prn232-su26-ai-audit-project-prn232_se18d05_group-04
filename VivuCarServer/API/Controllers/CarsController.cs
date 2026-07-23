using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Car;

namespace API.Controllers;

[ApiController]
[Route("api/cars")]
public class CarsController(ICarService carService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(SearchCarsPagedResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCars(
        [FromQuery] CarSearchQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await carService.SearchCarsAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("featured")]
    [ProducesResponseType(typeof(IReadOnlyList<CarSearchItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeaturedCars(
        [FromQuery] string? sortBy, 
        [FromQuery] int limit = 6,
        CancellationToken cancellationToken = default
    )
    {
        var result = await carService.GetFeaturedCarsAsync(sortBy, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSearchSuggestions(
        [FromQuery] string q,
        CancellationToken cancellationToken
    )
    {
        var result = await carService.GetSearchSuggestionsAsync(q, cancellationToken);
        return Ok(result);
    }

    [HttpGet("filter-options")]
    [ProducesResponseType(typeof(CarFilterOptionsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
    {
        var result = await carService.GetFilterOptionsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CarDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCarDetail(
        int id,
        CancellationToken cancellationToken
    )
    {
        var result = await carService.GetCarDetailByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new { message = $"Car with ID {id} not found." });
        }
        return Ok(result);
    }
}
