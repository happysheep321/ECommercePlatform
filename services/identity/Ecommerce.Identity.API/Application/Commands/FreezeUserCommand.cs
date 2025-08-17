using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class FreezeUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
