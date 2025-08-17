using ECommerce.SharedKernel.Enums;

namespace ECommerce.Identity.API.Application.DTOs
{
    /// <summary>
    /// 用户列表DTO - 管理员查看用户列表使用
    /// </summary>
    public class UserListDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 角色列表（简化信息）
        /// </summary>
        public List<string> RoleNames { get; set; } = new List<string>();

        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否被冻结
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// 是否被封禁
        /// </summary>
        public bool IsBanned { get; set; }
    }
}
