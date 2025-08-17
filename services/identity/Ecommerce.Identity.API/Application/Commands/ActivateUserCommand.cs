using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class ActivateUserCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
