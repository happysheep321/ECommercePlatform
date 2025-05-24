using Ecommerce.Identity.API.Domain.SeedWork;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class User : IAggregateRoot
    {
        public required string UserName { get; set; }

        public string Passord { get; set; }

        public string Email { get; set; }

        private readonly List<Guid> roleIds = new();

        public IReadOnlyCollection<Guid> RoleIds => roleIds.AsReadOnly();

        public void AddRole(Guid roleId)
        {
            if (roleIds.Contains(roleId))
                return;
            roleIds.Add(roleId);
        }

        public void RemoveRole(Guid roleId)
        {
            if (!roleIds.Contains(roleId))
                return;
            roleIds.Remove(roleId);
        }
    }
}
