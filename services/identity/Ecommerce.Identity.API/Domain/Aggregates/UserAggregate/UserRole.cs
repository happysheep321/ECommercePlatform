using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = default!;
    }
}
