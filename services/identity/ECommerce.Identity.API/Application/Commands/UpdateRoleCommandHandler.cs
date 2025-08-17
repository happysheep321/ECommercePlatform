using MediatR;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
    {
        private readonly IRoleService roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            await roleService.UpdateRoleAsync(request);
        }
    }
}
