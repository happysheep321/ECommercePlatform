using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class DisableRoleCommand : IRequest
    {
        public Guid RoleId { get; set; }
    }
}
