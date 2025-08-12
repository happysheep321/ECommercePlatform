namespace ECommerce.Identity.API.Application.Commands
{
    public class ChangePasswordCommand
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
