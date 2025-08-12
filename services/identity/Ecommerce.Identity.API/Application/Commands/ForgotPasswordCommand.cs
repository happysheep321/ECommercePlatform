namespace ECommerce.Identity.API.Application.Commands
{
    public class ForgotPasswordCommand
    {
        public string Email { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string EmailVerifyCode { get; set; } = default!;
    }
}
