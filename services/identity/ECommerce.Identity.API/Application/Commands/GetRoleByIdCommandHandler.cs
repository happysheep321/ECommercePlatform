using MediatR;
using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetRoleByIdCommandHandler : IRequestHandler<GetRoleByIdCommand, RoleDto?>
    {
        private readonly IRoleService roleService;

        public GetRoleByIdCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task<RoleDto?> Handle(GetRoleByIdCommand request, CancellationToken cancellationToken)
        {
            return await roleService.GetRoleByIdAsync(request);
        }
    }
}
