namespace Ecommerce.Identity.API.Application.Commands
{
    public class FreezeUserCommand
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
