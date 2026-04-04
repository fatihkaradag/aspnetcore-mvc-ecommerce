using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace aspnetcore_mvc_ecommerce.Utility
{
    // Sends emails via Mailtrap SMTP sandbox for development/testing
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        // Sends HTML email through Mailtrap sandbox SMTP
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _config["Mailtrap:Host"]!;
            var port = int.Parse(_config["Mailtrap:Port"]!);
            var username = _config["Mailtrap:Username"]!;
            var password = _config["Mailtrap:Password"]!;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@bookstore.com"), // Mailtrap sandbox accepts any valid format
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await client.SendMailAsync(mailMessage);
        }
    }
}