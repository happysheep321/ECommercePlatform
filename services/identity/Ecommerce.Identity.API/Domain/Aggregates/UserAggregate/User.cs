using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;
using ECommerce.SharedKernel.Enums;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class User : Entity<Guid>, IAggregateRoot
    {
        public required string UserName { get; set; }

        public required string PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public UserType Type { get; set; } = UserType.Buyer;

        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime RegisterTime { get; set; } = DateTime.UtcNow;

        // ==== 用户扩展资料 ====
        public UserProfile Profile { get; private set; } = new UserProfile("新用户", "", DateTime.UtcNow, Gender.Unspecified);

        public void UpdateProfile(UserProfile newProfile)
        {
            Profile = newProfile;
        }

        // ==== 用户角色 ====
        private readonly List<UserRole> userRoles = new();

        public IReadOnlyCollection<UserRole> UserRoles => userRoles.AsReadOnly();

        public void AddRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (userRoles.Any(ur => ur.RoleId == role.Id))
                return; // 已有该角色，不重复添加

            userRoles.Add(new UserRole
            {
                User = this,
                UserId = this.Id,
                Role = role,
                RoleId = role.Id
            });
        }

        public void RemoveRole(Guid roleId)
        {
            var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole != null)
                userRoles.Remove(userRole);
        }

        // ==== 用户地址 ====
        private readonly List<UserAddress> addresses = new();

        public IReadOnlyCollection<UserAddress> Addresses => addresses.AsReadOnly();

        public void AddAddress(UserAddress addAddress)
        {
            if (!addresses.Any())
            {
                addAddress.SetDefault(true);
            }
            else if (addAddress.IsDefault)
            {
                foreach (var addr in addresses)
                {
                    addr.SetDefault(false); // 取消之前的默认地址
                }
            }

            addresses.Add(addAddress);
        }

        public void UpdateAddress(UserAddress updateAddress)
        {
            var existAddress = addresses.FirstOrDefault(a=>a.Id==updateAddress.Id);
            if (existAddress == null)
            {
                throw new InvalidOperationException("该地址不存在");
            }

            existAddress.UpdateAddress(
                updateAddress.ReceiverName,
                updateAddress.Phone,
                updateAddress.Region,
                updateAddress.Detail,
                updateAddress.IsDefault);

            if (updateAddress.IsDefault && !existAddress.IsDefault)
            {
                foreach (var addr in addresses.Where(a => a.Id != existAddress.Id))
                {
                    addr.SetDefault(false);
                }
            }
        }

        public void RemoveAddress(UserAddress removeAddress)
        {
            var address=addresses.FirstOrDefault(a=>a.Id== removeAddress.Id);
            if (address == null)
            {
                return;
            }

            if (address.IsDefault && addresses.Count > 1)
            {
                throw new InvalidOperationException("不能删除默认地址，请先设置其他地址为默认地址");
            }

            addresses.Remove(address);
        }

        // ==== 构造函数 ====
        /// <summary>
        /// EF Core无参构造函数
        /// </summary>
        protected User() { }

        /// <summary>
        /// 领域构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passwordHash">密码</param>
        /// <param name="email">邮件</param>
        public User(string userName, string passwordHash, string email)
        {
            Id = Guid.NewGuid(); //聚合根生成的ID
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
        }
    }
}
