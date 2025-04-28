using E_CommerceAPI.ENTITES.Helpers;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;




namespace E_CommerceAPI.SERVICES.Implementation
{
    public class MailService : IMailService
    {
        #region Fields


        private readonly EmailSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MailService> _logger;
        private readonly IUrlHelper _urlHelper;

        #endregion


        public MailService(IUnitOfWork unitOfWork,
                           IOptions<EmailSettings> setting,
                           UserManager<ApplicationUser> userManager,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<MailService> logger,
                           IUrlHelper urlHelper)
        {
            _settings = setting.Value;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _urlHelper = urlHelper;
        }

        public async Task<string> ConfirmEmailAsync(ApplicationUser user, string subject)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("Cannot send confirmation email: User or user's email is null or empty.");
                return "User or user's email cannot be null or empty.";
            }

            if (string.IsNullOrEmpty(_settings.Email) || string.IsNullOrEmpty(_settings.Host) || string.IsNullOrEmpty(_settings.Password))
            {
                _logger.LogWarning("Email settings are not properly configured.");
                return "Email settings are not properly configured.";
            }

            try
            {
                // Generate email confirmation token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Build confirmation URL
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var returnUrl = $"{baseUrl}/api/Account/ConfirmEmail?UserId={user.Id}&Code={Uri.EscapeDataString(code)}";
                Console.WriteLine(returnUrl);

                // Create email message
                var emailMessage = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_settings.Email),
                    Subject = subject,
                    To = { MailboxAddress.Parse(user.Email) },
                    From = { new MailboxAddress(_settings.DisplayName, _settings.Email) }
                };

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = ConfirmBodyGenerator(user.FirstName, returnUrl)
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                // Send email using SMTP
                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_settings.Email, _settings.Password);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation($"Confirmation email sent successfully to {user.Email}");
                return "Email Confirmed Successfully";
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                _logger.LogError(ex, $"Failed to send confirmation email to {user.Email}. SMTP Error.");
                throw new InvalidOperationException("Failed to send email due to an SMTP issue.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while sending confirmation email to {user.Email}.");
                throw new InvalidOperationException("An unexpected error occurred while sending the email.", ex);
            }
        }
        private string GenerateOtpCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<string> SendOtpAsync(ApplicationUser user, string subject)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("Cannot send OTP: User or user's email is null or empty.");
                return "User or user's email cannot be null or empty.";
            }

            if (string.IsNullOrEmpty(_settings.Email) || string.IsNullOrEmpty(_settings.Host) || string.IsNullOrEmpty(_settings.Password))
            {
                _logger.LogWarning("Email settings are not properly configured.");
                return "Email settings are not properly configured.";
            }

            try
            {
                // Generate a random OTP code
                var otpCode = GenerateOtpCode(6); // 6-digit OTP

                // Update the user's OTP in the database
                user.Code = otpCode;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update the user in the database.");
                    return "Error updating user in the database.";
                }

                // Create email message
                var emailMessage = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_settings.Email),
                    Subject = subject,
                    To = { MailboxAddress.Parse(user.Email) },
                    From = { new MailboxAddress(_settings.DisplayName, _settings.Email) }
                };

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = OtpBodyGenerator(user.FirstName, otpCode)
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                // Send email using SMTP
                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_settings.Email, _settings.Password);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation($"OTP sent successfully to {user.Email}");
                return "OTP sent successfully!";
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                _logger.LogError(ex, $"Failed to send OTP to {user.Email}. SMTP Error.");
                throw new InvalidOperationException("Failed to send OTP due to an SMTP issue.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while sending OTP to {user.Email}.");
                throw new InvalidOperationException("An unexpected error occurred while sending the OTP.", ex);
            }
        }

        private string OtpBodyGenerator(string firstName, string otpCode)
        {
            return $@"
        <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 30px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }}
                    .header {{
                        text-align: center;
                        padding-bottom: 20px;
                    }}
                    .header img {{
                        width: 100px;
                    }}
                    h2 {{
                        color: #333;
                    }}
                    p {{
                        font-size: 16px;
                        color: #555;
                        line-height: 1.5;
                    }}
                    .otp-code {{
                        font-size: 24px;
                        font-weight: bold;
                        color: #2c3e50;
                        background-color: #e8f0fe;
                        padding: 10px 20px;
                        border-radius: 5px;
                        display: inline-block;
                        margin: 20px 0;
                    }}
                    .footer {{
                        margin-top: 40px;
                        font-size: 12px;
                        color: #aaa;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://moelandingstagesa.blob.core.windows.net/admin/2025-02-03//20250203T101527926.png' alt='Logo' />
                    </div>
                    <h2>Hello {firstName},</h2>
                    <p>We received a request to verify your account using a One-Time Password (OTP).</p>
                    <p>Your OTP is:</p>
                    <div class='otp-code'>{otpCode}</div>
                    <p>Please enter this code in the verification screen to complete the process.</p>
                    <p>If you did not request this, you can safely ignore this email.</p>
                    <div class='footer'>
                        &copy; {(DateTime.Now.Year)} Your Company Name. All rights reserved.
                    </div>
                </div>
            </body>
        </html>";
        }

        private string ConfirmBodyGenerator(string firstName, string confirmEmailUrl)
        {
            return $@"
        <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 30px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }}
                    .header {{
                        text-align: center;
                        padding-bottom: 20px;
                    }}
                    .header img {{
                        width: 120px;
                    }}
                    h2 {{
                        color: #333;
                    }}
                    p {{
                        font-size: 16px;
                        color: #555;
                    }}
                    .button {{
                        display: inline-block;
                        margin-top: 20px;
                        padding: 12px 25px;
                        background-color: #4a90e2;
                        color: white;
                        text-decoration: none;
                        border-radius: 5px;
                        font-weight: bold;
                    }}
                    .footer {{
                        margin-top: 40px;
                        font-size: 12px;
                        color: #aaa;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://moelandingstagesa.blob.core.windows.net/admin/2025-02-03//20250203T101527926.png' alt='Logo' width='120' />

                    </div>
                    <h2>Hello {firstName},</h2>
                    <p>Thanks for signing up to Ecommerce DEPI. To complete your registration, please confirm your email address by clicking the button below.</p>
                    <p>
                        <a href='{confirmEmailUrl}' class='button'>Confirm Email</a>
                    </p>
                    <p>If you didn't request this, you can safely ignore this email.</p>
                    <div class='footer'>
                        &copy; {(DateTime.Now.Year)} Ecommerce DEPI. All rights reserved.
                    </div>
                </div>
            </body>
        </html>";
        }






        #region SendEmail Service
        public async Task<string> SendEmail(string email, string Message, string? reason)
        {
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_settings.Host, _settings.Port, true);
                    client.Authenticate(_settings.Email, _settings.Password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = $"{Message}",
                        TextBody = "wellcome",
                    };
                    var message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    message.From.Add(new MailboxAddress("Ecommerce DEPI", _settings.Email));
                    message.To.Add(new MailboxAddress("testing", email));
                    message.Subject = reason == null ? "No Submitted" : reason;
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Failed {ex.Message}";
            }
        }

        #endregion
    }
}
