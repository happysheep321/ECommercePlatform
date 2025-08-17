using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 查询角色详情命令
    /// </summary>
    public class GetRoleByIdCommand : IRequest<RoleDto?>
    {
        public Guid RoleId { get; set; }
    }
}
