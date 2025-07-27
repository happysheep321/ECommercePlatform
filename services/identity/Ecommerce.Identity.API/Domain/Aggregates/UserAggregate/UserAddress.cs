using Ecommerce.Identity.API.Domain.ValueObjects;
using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserAddress : EntityBase<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; private set; }

        public User User { get; private set; } = default!;

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; private set; } = string.Empty;

        /// <summary>
        /// 收件人手机号
        /// </summary>
        public string Phone { get; private set; } = string.Empty;

        /// <summary>
        /// 区域
        /// </summary>
        public Region Region { get; private set; } = default!;

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Detail { get; private set; } = string.Empty;

        /// <summary>
        /// 是否为默认地址
        /// </summary>
        public bool IsDefault { get; private set; }

        protected UserAddress()
        {
            
        }

        public UserAddress(Guid userId, string receiverName, string phone, Region region, string detail, bool isDefault)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
            IsDefault = isDefault;
        }

        public UserAddress(Guid id, Guid userId, string receiverName, string phone, Region region, string detail, bool isDefault)
        {
            Id = id;
            UserId = userId;
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
            IsDefault = isDefault;
        }

        public void UpdateAddress(string receiverName, string phone, Region region, string detail)
        {
            ReceiverName = receiverName;
            Phone = phone;
            Region = region;
            Detail = detail;
        }

        public void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
