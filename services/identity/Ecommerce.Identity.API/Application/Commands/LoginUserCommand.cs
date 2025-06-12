namespace Ecommerce.Identity.API.Application.Commands
{
    public class LoginUserCommand
    {
        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}
