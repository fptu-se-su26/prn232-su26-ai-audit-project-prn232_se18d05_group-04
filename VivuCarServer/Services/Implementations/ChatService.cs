using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Chat;

namespace Services.Implementations;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGeminiClient _geminiClient;

    public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IGeminiClient geminiClient)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _geminiClient = geminiClient;
    }

    public async Task<ChatSessionResponse> GetOrCreateSessionAsync(int? userId, int? bookingId)
    {
        var activeSession = userId.HasValue ? await _chatRepository.GetActiveSessionByUserIdAsync(userId) : null;
        if (activeSession == null)
        {
            AppUser? user = null;
            if (userId.HasValue)
            {
                user = await _userRepository.FindByIdAsync(userId.Value);
            }
            
            activeSession = new ChatSession
            {
                UserId = userId,
                BookingId = bookingId,
                SessionType = "ai",
                Status = "open",
                CreatedAt = DateTimeOffset.UtcNow
            };
            activeSession = await _chatRepository.CreateSessionAsync(activeSession);
            activeSession.User = user!; // user can be null, the relationship allows it now
        }

        var response = MapToResponse(activeSession);
        var messages = await _chatRepository.GetMessagesBySessionIdAsync(activeSession.Id);
        response.Messages = messages.Select(MapToResponse).ToList();
        return response;
    }

    public async Task<ChatSessionResponse?> GetSessionByIdAsync(int sessionId)
    {
        var session = await _chatRepository.GetSessionByIdAsync(sessionId);
        if (session == null) return null;

        var response = MapToResponse(session);
        var messages = await _chatRepository.GetMessagesBySessionIdAsync(sessionId);
        response.Messages = messages.Select(MapToResponse).ToList();
        return response;
    }

    public async Task<ChatMessageResponse> SendMessageAsync(int sessionId, int? senderId, string role, string content)
    {
        var message = new ChatMessage
        {
            SessionId = sessionId,
            SenderId = senderId,
            Role = role,
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _chatRepository.AddMessageAsync(message);
        
        var senderName = "System";
        if (role == "assistant") senderName = "VivuCar Assistant";
        else if (senderId.HasValue)
        {
            var user = await _userRepository.FindByIdAsync(senderId.Value);
            if (user != null) senderName = user.FullName;
        }

        return new ChatMessageResponse
        {
            Id = message.Id,
            SessionId = message.SessionId,
            SenderId = message.SenderId,
            Role = message.Role,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            SenderName = senderName
        };
    }

    public async Task<ChatMessageResponse> HandleAiMockResponseAsync(int sessionId, string userMessage)
    {
        var session = await _chatRepository.GetSessionByIdAsync(sessionId);
        if (session == null) return null!;

        // Fetch history
        var history = await _chatRepository.GetMessagesBySessionIdAsync(sessionId);
        var mappedHistory = history.Select(MapToResponse).ToList();

        // System Prompt
        var systemPrompt = @"Bạn là trợ lý AI (VivuCar Assistant) của hệ thống thuê xe tự lái VivuCar tại Đà Nẵng.
        Nhiệm vụ của bạn là hỗ trợ khách hàng tìm xe, giải đáp thắc mắc về chính sách, và tra cứu thông tin chuyến đi.
        
        Quy tắc:
        1. Luôn xưng 'tôi' và gọi khách là 'bạn'.
        2. Nếu khách hỏi thông tin chuyến đi (booking), hãy gọi công cụ get_user_bookings.
        3. Nếu khách muốn tìm xe, thuê xe, xem xe, hãy gọi công cụ get_car_list.
        4. Chính sách thanh toán: Khách phải cọc trước (VNPay/MoMo/Tiền mặt) sau đó hệ thống sẽ thông báo cho chủ xe để duyệt.
        5. Nếu khách gặp sự cố khẩn cấp hoặc bạn không thể trả lời, hãy khuyên khách yêu cầu 'Gặp người hỗ trợ' để tiếp quản.
        6. Luôn trả lời ngắn gọn, thân thiện và súc tích bằng tiếng Việt.";

        var aiResponseText = await _geminiClient.GetChatResponseAsync(mappedHistory, systemPrompt, session.UserId);

        return await SendMessageAsync(sessionId, null, "assistant", aiResponseText);
    }

    public async Task EscalateToLiveChatAsync(int sessionId)
    {
        var session = await _chatRepository.GetSessionByIdAsync(sessionId);
        if (session != null)
        {
            session.Status = "escalated"; // WAITING_OWNER
            session.EscalatedAt = DateTimeOffset.UtcNow;
            await _chatRepository.UpdateSessionAsync(session);
        }
    }

    public async Task<List<ChatSessionSummary>> GetOwnerConversationsAsync(string? status, string? priority, string? keyword)
    {
        var sessions = await _chatRepository.GetSessionsForOwnerAsync(status, priority, keyword);
        
        var result = new List<ChatSessionSummary>();
        foreach (var s in sessions)
        {
            var lastMsg = s.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
            result.Add(new ChatSessionSummary
            {
                Id = s.Id,
                CustomerName = s.User?.FullName ?? "Unknown",
                BookingId = s.BookingId,
                Status = s.Status == "open" && s.SessionType == "ai" ? "AI_ONLY" :
                         s.Status == "escalated" ? "WAITING_OWNER" : 
                         s.Status == "open" && s.SessionType == "live" ? "OWNER_JOINED" : "RESOLVED",
                LastMessage = lastMsg?.Content ?? "",
                LastMessageAt = lastMsg?.CreatedAt,
                Priority = priority ?? "MEDIUM",
                UnreadCount = 0 // Mock 0 for now
            });
        }
        return result;
    }

    public async Task TakeOverChatAsync(int sessionId, int ownerId)
    {
        var session = await _chatRepository.GetSessionByIdAsync(sessionId);
        if (session != null)
        {
            session.Status = "open";
            session.SessionType = "live";
            session.AssignedTo = ownerId;
            await _chatRepository.UpdateSessionAsync(session);
        }
    }

    public async Task ResolveChatAsync(int sessionId)
    {
        var session = await _chatRepository.GetSessionByIdAsync(sessionId);
        if (session != null)
        {
            session.Status = "closed";
            session.ClosedAt = DateTimeOffset.UtcNow;
            await _chatRepository.UpdateSessionAsync(session);
        }
    }

    public async Task<List<ChatMessageResponse>> GetChatHistoryAsync(int sessionId)
    {
        var messages = await _chatRepository.GetMessagesBySessionIdAsync(sessionId);
        return messages.Select(MapToResponse).ToList();
    }

    private ChatSessionResponse MapToResponse(ChatSession session)
    {
        return new ChatSessionResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            UserFullName = session.User?.FullName ?? "",
            BookingId = session.BookingId,
            SessionType = session.SessionType,
            Status = session.Status,
            AssignedTo = session.AssignedTo,
            CreatedAt = session.CreatedAt
        };
    }

    private ChatMessageResponse MapToResponse(ChatMessage message)
    {
        var senderName = "System";
        if (message.Role == "assistant") senderName = "VivuCar Assistant";
        else if (message.Sender != null) senderName = message.Sender.FullName;

        return new ChatMessageResponse
        {
            Id = message.Id,
            SessionId = message.SessionId,
            SenderId = message.SenderId,
            Role = message.Role,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            SenderName = senderName
        };
    }
}
