namespace Ecommerce.Identity.API.Application.Options
{
    public class EmailOptions
    {
        public string FromEmail { get; set; } = default!;
        public string DisplayName { get; set; } = "Ecommerce 验证服务";
        public string AuthCode { get; set; } = default!;
        public string SmtpHost { get; set; } = "smtp.qq.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
    }
}
