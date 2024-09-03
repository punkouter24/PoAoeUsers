using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using PoAoeUsers.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PoAoeUsers.Components.Account
{
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly string apiKey;
        private readonly IEmailSender emailSender;

        public IdentityNoOpEmailSender(string apiKey)
        {
            this.apiKey = apiKey;
            this.emailSender = new RealEmailSender(this.apiKey); // Initialize it here
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            return emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            return emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        }
    }

    internal class RealEmailSender : IEmailSender
    {
        private readonly string apiKey;

        public RealEmailSender(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendGridClient client = new(apiKey);
            EmailAddress from = new("punkouter24@gmail.com", "Punkouter Software");
             EmailAddress to = new(email);
            //EmailAddress to = new("punkouter24@gmail.com");
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            var response = await client.SendEmailAsync(msg);
        }
    }
}


