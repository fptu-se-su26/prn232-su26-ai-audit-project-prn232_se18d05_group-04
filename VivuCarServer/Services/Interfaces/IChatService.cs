using Services.Models.Chat;

namespace Services.Interfaces;

public interface IChatService
{
    Task<ChatSessionResponse> GetOrCreateSessionAsync(int? userId, int? bookingId);
    Task<ChatSessionResponse?> GetSessionByIdAsync(int sessionId);
    Task<ChatMessageResponse> SendMessageAsync(int sessionId, int? senderId, string role, string content);
    Task<ChatMessageResponse> HandleAiMockResponseAsync(int sessionId, string userMessage);
    Task EscalateToLiveChatAsync(int sessionId);
    Task<List<ChatSessionSummary>> GetOwnerConversationsAsync(string? status, string? priority, string? keyword);
    Task TakeOverChatAsync(int sessionId, int ownerId);
    Task ResolveChatAsync(int sessionId);
    Task<List<ChatMessageResponse>> GetChatHistoryAsync(int sessionId);
}
