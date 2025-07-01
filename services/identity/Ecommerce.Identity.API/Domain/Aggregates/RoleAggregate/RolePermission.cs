using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class RolePermission
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 关联的角色实体
        /// </summary>
        public Role Role { get; set; } = default!;

        /// <summary>
        /// 关联的权限实体
        /// </summary>
        public Permission Permission { get; set; } = default!;
    }
}
