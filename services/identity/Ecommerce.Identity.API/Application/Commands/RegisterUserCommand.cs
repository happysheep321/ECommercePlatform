using ECommerce.SharedKernel.Enums;

namespace Ecommerce.Identity.API.Application.Commands
{
    public class RegisterUserCommand
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public required string Phone { get; set; }

        /// <summary>
        /// 电子邮箱（可选）
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }
    }
}
