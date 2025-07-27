using Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate;
using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;
using Ecommerce.SharedKernel.Events;
using Ecommerce.Identity.API.Domain.Events;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class Role : EntityBase<Guid>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; } = default!;

        public bool Enabled { get; private set; }

        /// <summary>
        /// 是否为系统内置角色
        /// </summary>
        public bool IsSystemRole { get; private set; } = false;

        private readonly HashSet<RolePermission> rolePermissions = new();

        public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

        /// <summary>
        /// EF Core无参构造函数
        /// </summary>
        protected Role() { }

        /// <summary>
        /// 领域构造函数
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="name">角色名</param>
        /// <param name="description">角色描述</param>
        /// <param name="isSystemRole">是否为系统角色</param>
        public Role(Guid id, string name, string description, bool isSystemRole = false)
        {
            if(string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentNullException("角色名称不能为空",nameof(name));

            Id = id;
            Name = name;
            Description = description;
            IsSystemRole = isSystemRole;
            Enabled = true;
            rolePermissions = new HashSet<RolePermission>();
        }

        public void Enable()=>Enabled=true;

        public void Disable()=>Enabled=false;

        public bool AddPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));
            var rolePermission = new RolePermission(this.Id, permission.Id);
            if (!rolePermissions.Add(rolePermission))
                return false;
            AddDomainEvent(new RolePermissionGrantedEvent(this.Id, permission.Id));
            return true;
        }

        public bool RemovePermission(Guid permissionId)
        {
            var rolePermission = new RolePermission(this.Id, permissionId);
            if (rolePermissions.Remove(rolePermission))
            {
                AddDomainEvent(new RolePermissionRevokedEvent(this.Id, permissionId));
                return true;
            }
            return false;
        }

        public void ReplacePermissions(IEnumerable<Permission> newPermissions)
        {
            rolePermissions.Clear();

            foreach(var permission in newPermissions.DistinctBy(p => p.Id))
            {
                AddPermission(permission);
            }
        }
    }
}
