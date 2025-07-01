using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserRole
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户实体
        /// </summary>
        public User User { get; set; } = default!;

        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色实体
        /// </summary>
        public Role Role { get; set; } = default!;
    }
}
