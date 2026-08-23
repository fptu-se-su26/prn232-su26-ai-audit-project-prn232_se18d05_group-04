using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class ChatRepository : IChatRepository
{
    private readonly VivuCarDbContext _context;

    public ChatRepository(VivuCarDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession?> GetSessionByIdAsync(int id)
    {
        return await _context.ChatSessions
            .Include(s => s.User)
            .Include(s => s.Booking)
            .Include(s => s.AssignedOwner)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ChatSession?> GetActiveSessionByUserIdAsync(int? userId)
    {
        if (userId == null) return null;
        return await _context.ChatSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId.Value && s.Status != "closed");
    }

    public async Task<List<ChatSession>> GetSessionsForOwnerAsync(string? status, string? priority, string? keyword)
    {
        var query = _context.ChatSessions
            .Include(s => s.User)
            .Include(s => s.Messages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && status != "ALL")
        {
            if (status == "AI_ONLY") query = query.Where(s => s.Status == "open" && s.SessionType == "ai");
            else if (status == "WAITING_OWNER") query = query.Where(s => s.Status == "escalated");
            else if (status == "OWNER_JOINED") query = query.Where(s => s.Status == "open" && s.SessionType == "live");
            else if (status == "RESOLVED") query = query.Where(s => s.Status == "closed");
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(s => (s.User != null && s.User.FullName.ToLower().Contains(keyword)) || 
                                     (s.BookingId.HasValue && s.BookingId.Value.ToString().Contains(keyword)));
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<ChatSession> CreateSessionAsync(ChatSession session)
    {
        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateSessionAsync(ChatSession session)
    {
        _context.ChatSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId)
    {
        return await _context.ChatMessages
            .Include(m => m.Sender)
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
