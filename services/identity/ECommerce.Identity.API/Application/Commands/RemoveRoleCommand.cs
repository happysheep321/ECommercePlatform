using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class RemoveRoleCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
