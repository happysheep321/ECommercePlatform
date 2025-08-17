using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class RemovePermissionCommand : IRequest
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
