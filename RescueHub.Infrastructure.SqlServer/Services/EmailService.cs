using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RescueHub.Domain.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RescueHub.Infrastructure.SqlServer.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger; 

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var server = _configuration["EmailSettings:Server"];
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var password = _configuration["EmailSettings:Password"];

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
                _logger.LogError(ex, "Lỗi khi gửi email đến địa chỉ: {Email}", toEmail);
                return false;
            }
        }
    }
}