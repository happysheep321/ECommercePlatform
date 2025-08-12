namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 删除角色命令
    /// </summary>
    public class DeleteRoleCommand
    {
        public Guid RoleId { get; set; } // 要删除的角色ID
    }
}
