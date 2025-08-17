using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class DisableRoleCommandHandler : IRequestHandler<DisableRoleCommand>
    {
        private readonly IRoleService roleService;

        public DisableRoleCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Handle(DisableRoleCommand request, CancellationToken cancellationToken)
        {
            await roleService.DisableAsync(request);
        }
    }
}
