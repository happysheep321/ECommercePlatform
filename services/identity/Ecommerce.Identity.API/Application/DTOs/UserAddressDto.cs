namespace Ecommerce.Identity.API.Application.DTOs
{
    public class UserAddressDto
    {
        public Guid AddressId { get; set; }
        public string Recipient { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Province { get; set; } = default!;
        public string City { get; set; } = default!;
        public string District { get; set; } = default!;
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }
}
