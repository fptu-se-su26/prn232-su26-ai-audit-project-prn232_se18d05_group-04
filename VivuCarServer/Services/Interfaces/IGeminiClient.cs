using Services.Models.Chat;

namespace Services.Interfaces;

public interface IGeminiClient
{
    Task<string> GetChatResponseAsync(List<ChatMessageResponse> history, string systemPrompt, int? userId = null);
}
