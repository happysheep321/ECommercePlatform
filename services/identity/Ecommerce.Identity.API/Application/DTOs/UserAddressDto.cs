using ECommerce.Identity.API.Domain.ValueObjects;

namespace ECommerce.Identity.API.Application.DTOs
{
    /// <summary>
    /// �û���ַ DTO��������ǰ�˷����û���ַ��Ϣ
    /// </summary>
    public class UserAddressDto
    {
        /// <summary>
        /// ��ַΨһ��ʶ
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// �ռ�����������Ӧʵ���е� ReceiverName
        /// </summary>
        public string ReceiverName { get; set; } = default!;

        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        public string Phone { get; set; } = default!;

        /// <summary>
        /// ������ʡ-��-��ƴ���ַ���������Ӧʵ���е� Region �ֶ�
        /// </summary>
        public Region Region { get; set; } = default!;

        /// <summary>
        /// ��ϸ��ַ
        /// </summary>
        public string Detail { get; set; } = default!;

        /// <summary>
        /// �Ƿ�ΪĬ�ϵ�ַ
        /// </summary>
        public bool IsDefault { get; set; }
    }
}