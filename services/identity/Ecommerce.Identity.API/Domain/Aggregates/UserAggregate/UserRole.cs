namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserRole
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        // EF Core 需要的无参构造
        private UserRole() { }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        // 为 HashSet/List 的 Contains 去重提供支持
        public override bool Equals(object? obj)
        {
            if (obj is not UserRole other) return false;
            return UserId == other.UserId && RoleId == other.RoleId;
        }

        public override int GetHashCode() => HashCode.Combine(UserId, RoleId);
    }

}
