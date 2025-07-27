using Ecommerce.Identity.API.Domain.Aggregates.RoleAggregate;
using Ecommerce.Identity.API.Domain.Events;
using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;
using Ecommerce.SharedKernel.Interfaces;
using ECommerce.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class User : EntityBase<Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = default!;

        /// <summary>
        /// 密码哈希
        /// </summary>
        public string PasswordHash { get; set; } = default!;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; } = default!;

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType Type { get; set; } = UserType.Normal;

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Active;

        /// <summary>
        /// 注册时间（UTC）
        /// </summary>
        public DateTime RegisterTime { get; set; } = DateTime.UtcNow;

        // ==== 用户扩展资料 ====
        public UserProfile Profile { get; private set; } = default!;

        public void UpdateProfile(UserProfile newProfile)
        {
            Profile = newProfile;
        }

        // ==== 用户角色 ====
        private readonly HashSet<UserRole> userRoles = new();

        public IReadOnlyCollection<UserRole> UserRoles => userRoles;

        public void AddRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            var userRole = new UserRole(this.Id, role.Id);
            if (userRoles.Contains(userRole))
                return;
            userRoles.Add(userRole);
            AddDomainEvent(new UserRoleAssignedEvent(this.Id, role.Id));
        }

        public void RemoveRole(Guid roleId)
        {
            var userRole = new UserRole(this.Id, roleId);
            if (userRoles.Remove(userRole))
            {
                AddDomainEvent(new UserRoleRemovedEvent(this.Id, roleId));
            }
        }

        // ==== 用户地址 ====
        private readonly List<UserAddress> addresses = new();

        public IReadOnlyCollection<UserAddress> Addresses => addresses.AsReadOnly();

        public void AddAddress(UserAddress addAddress)
        {
            if (addresses.Any(a => a.ReceiverName == addAddress.ReceiverName && a.Detail == addAddress.Detail))
            {
                throw new InvalidOperationException("不能添加重复的地址");
            }

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
                updateAddress.Region!,
                updateAddress.Detail);

            if (updateAddress.IsDefault)
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
        protected User() { }

        public User(Guid id, string userName, string passwordHash, string email,string phone, UserType userType)
        {
            if (string.IsNullOrWhiteSpace(userName)) 
                throw new ArgumentException("用户名不能为空", nameof(userName));
            if (string.IsNullOrWhiteSpace(passwordHash)) 
                throw new ArgumentException("密码不能为空", nameof(passwordHash));

            Id = id; //聚合根生成的ID
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            PhoneNumber = phone;
            Type = userType;
            Profile = new UserProfile(
                nickName: $"新用户_{userName}",
                avatarUrl: "",
                birthday: DateTime.UtcNow,
                gender: Gender.Unspecified
            );
        }
    }
}
