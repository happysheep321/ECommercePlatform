using ECommerce.Identity.API.Domain.ValueObjects;

namespace ECommerce.Identity.API.Application.DTOs
{
    /// <summary>
    /// 用户地址 DTO，用于向前端返回用户地址信息
    /// </summary>
    public class UserAddressDto
    {
        /// <summary>
        /// 地址唯一标识
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// 收件人姓名，对应实体中的 ReceiverName
        /// </summary>
        public string ReceiverName { get; set; } = default!;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; } = default!;

        /// <summary>
        /// 地区（省-市-区拼接字符串），对应实体中的 Region 字段
        /// </summary>
        public Region Region { get; set; } = default!;

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Detail { get; set; } = default!;

        /// <summary>
        /// 是否为默认地址
        /// </summary>
        public bool IsDefault { get; set; }
    }
}