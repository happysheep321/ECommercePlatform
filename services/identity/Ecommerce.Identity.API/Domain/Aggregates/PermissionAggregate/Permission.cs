using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;
using ECommerce.SharedKernel.Enums;

namespace Ecommerce.Identity.API.Domain.Aggregates.PermissionAggregate
{
    public class Permission : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// 唯一标识，例如 "Permission:Order.Create"、"Page:Dashboard.View"
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 权限显示名称
        /// </summary>
        public string? DisplayName {  get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled {  get; private set; }

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
            Enabled = true;
        }

        public void Enable() => Enabled = true;

        public void Disable() => Enabled = false;
    }
}
