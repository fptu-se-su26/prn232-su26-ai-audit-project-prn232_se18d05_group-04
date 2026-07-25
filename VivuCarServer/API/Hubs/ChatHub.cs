using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;
using Services.Models.Chat;

namespace API.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinSession(string sessionIdStr)
    {
        if (int.TryParse(sessionIdStr, out int sessionId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
        }
    }

    public async Task LeaveSession(string sessionIdStr)
    {
        if (int.TryParse(sessionIdStr, out int sessionId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
        }
    }

    // This method is called from the Frontend directly
    public async Task SendMessage(string sessionIdStr, string content, int? senderId, string role)
    {
        if (int.TryParse(sessionIdStr, out int sessionId))
        {
            var msg = await _chatService.SendMessageAsync(sessionId, senderId, role, content);
            
            // Broadcast to everyone in the room (including sender to confirm)
            await Clients.Group(sessionId.ToString()).SendAsync("ReceiveMessage", msg);

            // If user sends message and it's an AI session, trigger AI response
            if (role == "user")
            {
                var session = await _chatService.GetSessionByIdAsync(sessionId);
                if (session != null && session.SessionType == "ai" && session.Status == "open")
                {
                    // Trigger AI
                    var aiMsg = await _chatService.HandleAiMockResponseAsync(sessionId, content);
                    if (aiMsg != null)
                    {
                        await Clients.Group(sessionId.ToString()).SendAsync("ReceiveMessage", aiMsg);
                    }
                }
            }
        }
    }
}
