using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IChatRepository
{
    Task<ChatSession?> GetSessionByIdAsync(int id);
    Task<ChatSession?> GetActiveSessionByUserIdAsync(int? userId);
    Task<List<ChatSession>> GetSessionsForOwnerAsync(string? status, string? priority, string? keyword);
    Task<ChatSession> CreateSessionAsync(ChatSession session);
    Task UpdateSessionAsync(ChatSession session);
    Task<ChatMessage> AddMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId);
}
