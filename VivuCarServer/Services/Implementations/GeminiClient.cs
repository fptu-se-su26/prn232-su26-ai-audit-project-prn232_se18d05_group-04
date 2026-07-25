using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Chat;

namespace Services.Implementations;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IBookingRepository _bookingRepository;
    private readonly ICarRepository _carRepository;

    public GeminiClient(HttpClient httpClient, IConfiguration config, IBookingRepository bookingRepository, ICarRepository carRepository)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] ?? "";
        _bookingRepository = bookingRepository;
        _carRepository = carRepository;
    }

    public async Task<string> GetChatResponseAsync(List<ChatMessageResponse> history, string systemPrompt, int? userId = null)
    {
        if (string.IsNullOrEmpty(_apiKey)) return "Hệ thống chưa được cấu hình API Key của AI.";

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

        var contents = new List<object>();

        // We can't directly use system instruction in content array in Gemini API, we use system_instruction object
        
        foreach (var msg in history)
        {
            if (msg.Role == "system") continue; // system prompt handled separately
            var role = msg.Role == "assistant" ? "model" : "user";
            contents.Add(new
            {
                role = role,
                parts = new[] { new { text = msg.Content } }
            });
        }

        // Tools for function calling
        var tools = new[]
        {
            new
            {
                functionDeclarations = new[]
                {
                    new
                    {
                        name = "get_user_bookings",
                        description = "Lấy danh sách các chuyến đi (bookings) của người dùng hiện tại.",
                        parameters = new
                        {
                            type = "OBJECT",
                            properties = new { }, // no parameters needed because we know the context userId
                        }
                    },
                    new
                    {
                        name = "get_car_list",
                        description = "Lấy danh sách xe đang có sẵn để thuê.",
                        parameters = new
                        {
                            type = "OBJECT",
                            properties = new { },
                        }
                    }
                }
            }
        };

        var requestBody = new
        {
            system_instruction = new
            {
                parts = new[] { new { text = systemPrompt } }
            },
            contents = contents,
            tools = tools
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Gemini API Error: {err}");
            return "Tôi đang gặp khó khăn khi kết nối với máy chủ AI. Xin vui lòng thử lại sau.";
        }

        var resJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(resJson);
        var root = doc.RootElement;
        
        if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
        {
            var parts = candidates[0].GetProperty("content").GetProperty("parts");
            if (parts.GetArrayLength() > 0)
            {
                var part = parts[0];
                if (part.TryGetProperty("functionCall", out var functionCall))
                {
                    var functionName = functionCall.GetProperty("name").GetString();
                    var functionId = functionCall.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    return await HandleFunctionCallAsync(functionName, functionId, userId, contents, systemPrompt, url, part);
                }
                
                if (part.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? "";
                }
            }
        }

        return "Tôi chưa thể trả lời câu hỏi này.";
    }

    private async Task<string> HandleFunctionCallAsync(string? functionName, string? functionId, int? userId, List<object> contents, string systemPrompt, string url, JsonElement originalPart)
    {
        string functionResponseStr = "";
        if (functionName == "get_user_bookings")
        {
            if (userId == null) functionResponseStr = "Người dùng chưa đăng nhập. Hãy yêu cầu người dùng đăng nhập để xem thông tin chuyến đi.";
            else
            {
                var bookings = await _bookingRepository.GetListAsync(userId.Value, null, 1, 10);
                if (bookings.Count == 0) functionResponseStr = "Người dùng chưa có chuyến đi nào.";
                else
                {
                    var sb = new StringBuilder();
                    foreach (var b in bookings)
                    {
                        sb.AppendLine($"- Mã {b.Id}: Từ {b.StartDateTime:dd/MM/yyyy} đến {b.EndDateTime:dd/MM/yyyy}, Tổng tiền: {b.TotalAmount:N0} VNĐ, Trạng thái: {b.Status}");
                    }
                    functionResponseStr = sb.ToString();
                }
            }
        }
        else if (functionName == "get_car_list")
        {
            var result = await _carRepository.SearchCarsAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 10);
            var cars = result.Items;
            var sb = new StringBuilder();
            foreach (var c in cars)
            {
                sb.AppendLine($"- {c.CarBrand?.Name} {c.CarModel?.Name} ({c.SeatCount} chỗ): {c.DailyPrice:N0} VNĐ/ngày. Điểm nhận xe: {c.Location}");
            }
            functionResponseStr = sb.ToString();
        }
        else
        {
            functionResponseStr = "Function not implemented.";
        }

        // Send function result back to Gemini
        // Append the exact part the model returned (including thoughtSignature and id)
        contents.Add(new
        {
            role = "model",
            parts = new[] { originalPart }
        });

        object functionResponseObj;
        if (!string.IsNullOrEmpty(functionId))
        {
            functionResponseObj = new { name = functionName, response = new { name = functionName, content = functionResponseStr }, id = functionId };
        }
        else
        {
            functionResponseObj = new { name = functionName, response = new { name = functionName, content = functionResponseStr } };
        }

        contents.Add(new
        {
            role = "user",
            parts = new[] { new { functionResponse = functionResponseObj } }
        });

        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = contents
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            var resJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resJson);
            var root = doc.RootElement;
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var parts = candidates[0].GetProperty("content").GetProperty("parts");
                if (parts.GetArrayLength() > 0 && parts[0].TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? "";
                }
            }
        }

        return "Có lỗi xảy ra khi xử lý thông tin từ hệ thống.";
    }
}
