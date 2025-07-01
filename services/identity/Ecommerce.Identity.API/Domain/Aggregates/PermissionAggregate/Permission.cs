using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;

namespace Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate
{
    public class Permission : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// 权限名称，例如 "ViewProduct"、"EditUser"
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
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
