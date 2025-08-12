using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Application.Options;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ECommerce.Identity.API.Application.Services
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
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(options.DisplayName, options.FromEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                
                // 连接到SMTP服务器
                await smtp.ConnectAsync(options.SmtpHost, options.Port, SecureSocketOptions.StartTls);
                
                // 认证
                await smtp.AuthenticateAsync(options.FromEmail, options.AuthCode);
                
                // 发送邮件
                await smtp.SendAsync(email);
                
                // 断开连接
                await smtp.DisconnectAsync(true);

                logger.LogInformation("邮件已发送到 {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "邮件发送失败：{Message}", ex.Message);
                return false;
            }
        }
    }
}
