using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class Role:Entity<Guid>,IAggregateRoot
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        private readonly List<RolePermission> rolePermissions = new();

        public IReadOnlyCollection<RolePermission> RolePermissions=> rolePermissions.AsReadOnly();

        /// <summary>
        /// EF Core无参构造函数
        /// </summary>
        protected Role() { }

        /// <summary>
        /// 领域构造函数
        /// </summary>
        /// <param name="name">角色名</param>
        /// <param name="description">角色描述</param>
        public Role(Guid id, string name, string description)
        {
            Id = id; // 聚合根生成的ID
            Name = name;
            Description = description;
        }

        public void AddPermission(Permission permission)
        {
            if (rolePermissions.Any(rp => rp.PermissionId == permission.Id)) return;
            rolePermissions.Add(new RolePermission 
            {
                Role=this,
                RoleId=Id,
                Permission=permission,
                PermissionId=permission.Id
            });
        }

        public void RemovePermission(Guid permissionId)
        {
            var rolePermission = rolePermissions.FirstOrDefault(rp=>rp.PermissionId == permissionId);
            if(rolePermission != null)
                rolePermissions.Remove(rolePermission);
        }
    }
}
