using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Review;

namespace API.Controllers;

[Route("api/reviews")]
[ApiController]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return int.TryParse(idClaim, out var id) ? id : 0;
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetReviewByBookingId(int bookingId, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        if (customerId == 0) return Unauthorized();

        var review = await reviewService.GetByBookingIdAsync(customerId, bookingId, cancellationToken);
        if (review == null)
        {
            return NotFound(new { message = "Review not found or you are not authorized to view it." });
        }

        return Ok(review);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")] // Only customers can leave a review
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        if (customerId == 0) return Unauthorized();

        try
        {
            var result = await reviewService.CreateReviewAsync(customerId, request, cancellationToken);
            return Ok(result);
        }
        catch (ReviewServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        if (customerId == 0) return Unauthorized();

        try
        {
            var result = await reviewService.UpdateReviewAsync(customerId, id, request, cancellationToken);
            return Ok(result);
        }
        catch (ReviewServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteReview(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCurrentUserId();
        if (customerId == 0) return Unauthorized();

        try
        {
            await reviewService.DeleteReviewAsync(customerId, id, cancellationToken);
            return NoContent();
        }
        catch (ReviewServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
