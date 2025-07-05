using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Ecommerce.Identity.API.Application.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions options;
        private readonly ILogger<SmtpEmailSender> logger;

        public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(options.SmtpHost, options.Port)
                {
                    Credentials = new NetworkCredential(options.FromEmail, options.AuthCode),
                    EnableSsl = options.EnableSsl,
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(options.FromEmail, options.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mail.To.Add(to);

                await smtpClient.SendMailAsync(mail);
                logger.LogInformation("📧 邮件已发送到 {To}", to);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ 邮件发送失败：{Message}", ex.Message);
                throw new ApplicationException("发送邮件失败，请稍后重试");
            }
        }
    }
}
