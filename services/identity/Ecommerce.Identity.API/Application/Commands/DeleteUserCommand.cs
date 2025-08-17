using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
