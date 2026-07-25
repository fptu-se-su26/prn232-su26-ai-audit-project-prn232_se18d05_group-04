using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Services.Interfaces;

namespace Services.Implementations;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        var senderEmail = configuration["EmailSettings:SenderEmail"]
                          ?? Environment.GetEnvironmentVariable("EMAIL_SENDER")
                          ?? throw new InvalidOperationException("EMAIL_SENDER is not configured.");

        var senderPassword = configuration["EmailSettings:Password"]
                             ?? Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
                             ?? throw new InvalidOperationException("EMAIL_PASSWORD is not configured.");

        var senderName = configuration["EmailSettings:SenderName"] ?? "VivuCar";
        var host = configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
        var portString = configuration["EmailSettings:Port"] ?? "587";
        var port = int.TryParse(portString, out var p) ? p : 587;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "VivuCar — Mã xác thực đăng ký";

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = $"""
                    <div style="font-family:Arial,sans-serif;max-width:480px;margin:0 auto;padding:32px 20px;">
                      <h2 style="color:#245b3e;margin-bottom:8px;">VivuCar</h2>
                      <p style="color:#39483f;font-size:16px;">Cảm ơn bạn đã đăng ký tài khoản VivuCar.</p>
                      <p style="color:#39483f;font-size:16px;">Mã xác thực của bạn là:</p>
                      <div style="background:#f4f6f3;border:2px dashed #397254;border-radius:8px;padding:20px;text-align:center;margin:24px 0;">
                        <span style="font-size:32px;font-weight:700;color:#245b3e;letter-spacing:8px;">{otpCode}</span>
                      </div>
                      <p style="color:#748078;font-size:14px;">Mã có hiệu lực trong 5 phút. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                    </div>
                    """
        };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail, senderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("OTP email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
            throw;
        }
    }
}
