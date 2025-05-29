using Ecommerce.Identity.API.Domain.SeedWork;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class User : Entity<Guid>, IAggregateRoot
    {
        public required string UserName { get; set; }

        public required string Passord { get; set; }

        public required string Email { get; set; }

        private readonly List<Guid> roleIds = new();

        public IReadOnlyCollection<Guid> RoleIds => roleIds.AsReadOnly();

        /// <summary>
        /// EF Core无参构造函数
        /// </summary>
        protected User() { }

        /// <summary>
        /// 领域构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">邮件</param>
        public User(string userName,string password,string email)
        {
            Id=Guid.NewGuid(); //聚合根生成的ID
            UserName = userName;
            Passord = password;
            Email = email;
        }

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
