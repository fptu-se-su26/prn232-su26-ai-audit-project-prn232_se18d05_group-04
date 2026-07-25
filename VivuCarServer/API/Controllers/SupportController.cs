using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Chat;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SupportController : ControllerBase
{
    private readonly IChatService _chatService;

    public SupportController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var userId = GetUserId();
        var session = await _chatService.GetOrCreateSessionAsync(userId, request.BookingId);
        return Ok(session);
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversation(int id)
    {
        var session = await _chatService.GetSessionByIdAsync(id);
        if (session == null) return NotFound();
        return Ok(session);
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(int id, [FromBody] SendMessageRequest request)
    {
        var userId = GetUserId();
        var msg = await _chatService.SendMessageAsync(id, userId, "user", request.Content);

        // If it's an AI session, trigger AI mock reply
        var session = await _chatService.GetSessionByIdAsync(id);
        if (session != null && session.SessionType == "ai" && session.Status == "open")
        {
            await _chatService.HandleAiMockResponseAsync(id, request.Content);
        }

        return Ok(msg);
    }

    [HttpPost("conversations/{id}/escalate")]
    public async Task<IActionResult> EscalateConversation(int id)
    {
        await _chatService.EscalateToLiveChatAsync(id);
        return Ok(new { message = "Cuộc trò chuyện đã được chuyển cho người hỗ trợ." });
    }

    private int? GetUserId()
    {
        var claim = User?.Claims?.FirstOrDefault(c => c.Type == "Id" || c.Type == "nameidentifier" || c.Type == "sub" || c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (claim != null && int.TryParse(claim.Value, out int id)) return id;
        return null;
    }
}

public class StartConversationRequest
{
    public int? BookingId { get; set; }
}

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
}
