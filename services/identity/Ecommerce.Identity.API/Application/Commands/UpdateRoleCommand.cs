namespace ECommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 更新角色命令
    /// </summary>
    public class UpdateRoleCommand
    {
        public Guid RoleId { get; set; } // 角色ID
        public string Name { get; set; } = string.Empty; // 新的角色名称
        public string Description { get; set; } = string.Empty; // 新的描述
    }
}
