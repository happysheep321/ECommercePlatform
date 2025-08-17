using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class BanUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
