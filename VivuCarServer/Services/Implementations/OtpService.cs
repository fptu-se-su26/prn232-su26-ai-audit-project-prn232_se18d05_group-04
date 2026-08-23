using Microsoft.Extensions.Configuration;
using Services.Interfaces;

namespace Services.Implementations;

public class OtpService(IConfiguration configuration) : IOtpService
{
    private int ExpiryMinutes
    {
        get
        {
            var value = configuration["OtpSettings:ExpiryMinutes"] ?? "5";
            return int.TryParse(value, out var minutes) ? minutes : 5;
        }
    }

    public string GenerateCode() =>
        Random.Shared.Next(100_000, 1_000_000).ToString();

    public DateTime GetExpiryTime() =>
        DateTime.UtcNow.AddMinutes(ExpiryMinutes);

    public bool IsExpired(DateTime expiredAt) =>
        DateTime.UtcNow > expiredAt;
}
