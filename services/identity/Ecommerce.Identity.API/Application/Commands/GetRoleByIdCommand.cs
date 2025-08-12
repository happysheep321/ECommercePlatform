namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 查询角色详情命令
    /// </summary>
    public class GetRoleByIdCommand
    {
        public Guid RoleId { get; set; }
    }
}
