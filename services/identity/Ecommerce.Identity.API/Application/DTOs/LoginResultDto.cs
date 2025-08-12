namespace ECommerce.Identity.API.Application.DTOs
{
    public class LoginResultDto
    {
        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = default!;

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? AvatarUrl { get; set; }
    }
}
