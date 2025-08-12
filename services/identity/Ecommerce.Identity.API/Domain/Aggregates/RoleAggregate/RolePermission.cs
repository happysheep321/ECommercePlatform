namespace ECommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        // EF Core 需要的无参构造函数
        private RolePermission() { }

        public RolePermission(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }

        // 用于集合去重（HashSet 或 List.Contains）
        public override bool Equals(object? obj)
        {
            if (obj is not RolePermission other) return false;
            return RoleId == other.RoleId && PermissionId == other.PermissionId;
        }

        public override int GetHashCode() => HashCode.Combine(RoleId, PermissionId);
    }
}
