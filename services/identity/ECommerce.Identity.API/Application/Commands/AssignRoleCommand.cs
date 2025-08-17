using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class AssignRoleCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
