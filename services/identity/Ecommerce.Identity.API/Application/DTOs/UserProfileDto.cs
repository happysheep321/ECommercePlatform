namespace Ecommerce.Identity.API.Application.DTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string? Email { get; set; }
        public string Phone { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public List<UserAddressDto> Addresses { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }
}
