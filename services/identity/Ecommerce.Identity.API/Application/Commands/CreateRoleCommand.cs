namespace Ecommerce.Identity.API.Application.Commands
{
    /// <summary>
    /// 创建角色命令
    /// </summary>
    public class CreateRoleCommand
    {
        public string Name { get; set; } = string.Empty; // 角色名称
        public string Description { get; set; } = string.Empty; // 描述
    }
}
