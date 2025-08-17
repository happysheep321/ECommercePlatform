using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class AssignPermissionsCommand : IRequest
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
