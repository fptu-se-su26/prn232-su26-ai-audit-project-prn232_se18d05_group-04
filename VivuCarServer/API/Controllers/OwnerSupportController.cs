using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Chat;

namespace API.Controllers;

[ApiController]
[Route("api/owner/support")]
[Authorize] // Should require 'car_owner' or 'admin'
public class OwnerSupportController : ControllerBase
{
    private readonly IChatService _chatService;

    public OwnerSupportController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] string? status, [FromQuery] string? priority, [FromQuery] string? keyword)
    {
        var sessions = await _chatService.GetOwnerConversationsAsync(status, priority, keyword);
        return Ok(sessions);
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversation(int id)
    {
        var session = await _chatService.GetSessionByIdAsync(id);
        if (session == null) return NotFound();
        return Ok(session);
    }

    [HttpPost("conversations/{id}/take-over")]
    public async Task<IActionResult> TakeOverConversation(int id)
    {
        var ownerId = GetUserId();
        await _chatService.TakeOverChatAsync(id, ownerId);
        return Ok(new { message = "Bạn đã tiếp quản hội thoại này." });
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(int id, [FromBody] SendMessageRequest request)
    {
        var ownerId = GetUserId();
        var msg = await _chatService.SendMessageAsync(id, ownerId, "assistant", request.Content); // Owner replies as assistant or owner role
        return Ok(msg);
    }

    [HttpPost("conversations/{id}/resolve")]
    public async Task<IActionResult> ResolveConversation(int id)
    {
        await _chatService.ResolveChatAsync(id);
        return Ok(new { message = "Đã đánh dấu hoàn tất." });
    }

    [HttpGet("conversations/{id}/history")]
    public async Task<IActionResult> GetChatHistory(int id)
    {
        var history = await _chatService.GetChatHistoryAsync(id);
        return Ok(history);
    }

    private int GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "Id" || c.Type == "nameidentifier");
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
