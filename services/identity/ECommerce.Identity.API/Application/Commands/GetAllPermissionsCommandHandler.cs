using MediatR;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetAllPermissionsCommandHandler : IRequestHandler<GetAllPermissionsCommand, List<PermissionDto>>
    {
        private readonly IPermissionService permissionService;

        public GetAllPermissionsCommandHandler(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        public async Task<List<PermissionDto>> Handle(GetAllPermissionsCommand request, CancellationToken cancellationToken)
        {
            var permissions = await permissionService.GetAllPermissionsAsync();
            return permissions.ToList();
        }
    }
}
