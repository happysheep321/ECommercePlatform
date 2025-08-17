using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetAllPermissionsCommand : IRequest<List<PermissionDto>>
    {
    }
}
