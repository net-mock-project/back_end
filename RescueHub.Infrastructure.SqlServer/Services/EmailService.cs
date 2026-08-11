using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // 1. Thêm namespace này
using RescueHub.Domain.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RescueHub.Infrastructure.SqlServer.Services // (Hoặc namespace hiện tại của bạn)
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger; // 2. Khai báo biến logger

        // 3. Inject ILogger vào constructor
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var server = _configuration["SmtpSettings:Server"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var password = _configuration["SmtpSettings:Password"];

                using var client = new SmtpClient(server, port)
                {
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // 4. Giờ thì _logger đã hoạt động bình thường
                _logger.LogError(ex, "Lỗi khi gửi email đến địa chỉ: {Email}", toEmail);
                return false;
            }
        }
    }
}