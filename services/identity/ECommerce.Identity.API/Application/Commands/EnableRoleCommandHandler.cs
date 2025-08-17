using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class EnableRoleCommandHandler : IRequestHandler<EnableRoleCommand>
    {
        private readonly IRoleService roleService;

        public EnableRoleCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Handle(EnableRoleCommand request, CancellationToken cancellationToken)
        {
            await roleService.EnableAsync(request);
        }
    }
}
