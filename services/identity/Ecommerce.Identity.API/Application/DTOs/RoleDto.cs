namespace Ecommerce.Identity.API.Application.DTOs
{
    /// <summary>
    /// 用户角色 DTO，用于返回角色基本信息
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 角色描述（可选）
        /// </summary>
        public string? Description { get; set; }
    }
}
