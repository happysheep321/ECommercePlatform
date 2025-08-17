using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand>
    {
        private readonly IRoleService roleService;

        public AssignPermissionsCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            await roleService.AssignPermissionsToRoleAsync(request.RoleId, request.PermissionIds);
        }
    }
}
