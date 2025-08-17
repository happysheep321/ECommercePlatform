using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class RemovePermissionCommandHandler : IRequestHandler<RemovePermissionCommand>
    {
        private readonly IRoleService roleService;

        public RemovePermissionCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Handle(RemovePermissionCommand request, CancellationToken cancellationToken)
        {
            await roleService.RemovePermissionFromRoleAsync(request.RoleId, request.PermissionId);
        }
    }
}
