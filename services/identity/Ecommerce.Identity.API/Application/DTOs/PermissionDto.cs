namespace ECommerce.Identity.API.Application.DTOs
{
    /// <summary>
    /// 权限信息 DTO
    /// </summary>
    public class PermissionDto
    {
        public Guid PermissionId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!; // 用于权限判断
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsSystemPermission { get; set; } = false;
    }
}
