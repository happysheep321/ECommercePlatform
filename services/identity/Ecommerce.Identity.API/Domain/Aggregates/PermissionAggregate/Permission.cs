using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;

namespace Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate
{
    public class Permission : Entity<Guid>, IAggregateRoot
    {
        public string Name { get; set; } // "ViewProduct", "EditUser"
        public string? Description { get; set; }

        public Permission() { }

        public Permission(Guid id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
