using Ecommerce.Identity.API.Domain.SeedWork;

namespace Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate
{
    public class Role:Entity<Guid>,IAggregateRoot
    {
        public required string Name { get; set; }

        public string? Desciption { get; set; }

        /// <summary>
        /// EF Core无参构造函数
        /// </summary>
        public Role() { }

        /// <summary>
        /// 领域构造函数
        /// </summary>
        /// <param name="name">角色名</param>
        /// <param name="description">角色描述</param>
        public Role(string name, string description)
        {
            Id = Guid.NewGuid(); // 聚合根生成的ID
            Name = name;
            Desciption = description;
        }
    }
}
