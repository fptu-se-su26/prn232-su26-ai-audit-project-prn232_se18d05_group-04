namespace Services.Models.Chat;

public class ChatSessionResponse
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public string SessionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? AssignedTo { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ChatMessageResponse> Messages { get; set; } = new();
}

public class ChatMessageResponse
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int? SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class ChatSessionSummary
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public DateTimeOffset? LastMessageAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = "MEDIUM";
    public int UnreadCount { get; set; }
}
