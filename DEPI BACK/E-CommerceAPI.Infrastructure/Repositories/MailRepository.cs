using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.Common;
using E_CommerceAPI.Infrastructure.Context;
using E_CommerceAPI.Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace E_CommerceAPI.Infrastructure.Repositories
{
    public class MailRepository : GenericRepository<EmailSettings>, IMailRepository
    {
        private readonly EmailSettings _settings;

        public MailRepository(ECommerceDbContext context, IOptions<EmailSettings> setting) : base(context)
        {
            _settings = setting.Value;
        }

        private string ResetPasswordBodyGenerator(string name, string ChangePassPage)
        {
            string body = "<div>";
            body += "<h3>Hello " + name + ",</h3>";
            body += "<h5>If you need to reset your password click: </h5>";
            body += "<h5>" + ChangePassPage + ".</h5>";
            body += "<h4>Have a nice day,</h4>";
            body += "<h6>API-Commerce Support Team.</h6>";
            body += "</div>";
            return body;
        }

        public async Task SendResetPasswordEmailAsync(ApplicationUser user, string subject)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_settings.Email),
                Subject = subject,
            };

            email.To.Add(MailboxAddress.Parse(user.Email));
            var builder = new BodyBuilder();
            string ChangePassPage = "https://localhost:44321/api/Account/ResetPassword";

            builder.HtmlBody = ResetPasswordBodyGenerator(user.FirstName, ChangePassPage);
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_settings.Email, _settings.Password);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }
    }
}
