using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public required User User { get; set; }

        public Guid RoleId { get; set; }
        public required Role Role { get; set; }
    }
}
