using Microsoft.AspNetCore.Identity.UI.Services;

namespace aspnetcore_mvc_ecommerce.Utility
{
    // Placeholder email sender — replace with real email service before production
    // TODO: Integrate with a real email service provider (e.g. SendGrid, Mailgun, SMTP)
    public class EmailSender : IEmailSender
    {
        // Sends an email — currently a no-op, returns completed task
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}