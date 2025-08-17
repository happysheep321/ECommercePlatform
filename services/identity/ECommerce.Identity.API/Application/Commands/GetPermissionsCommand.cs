using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetPermissionsCommand : IRequest<List<PermissionDto>>
    {
        public Guid RoleId { get; set; }
    }
}
