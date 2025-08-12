using ECommerce.SharedKernel.Enums;

namespace ECommerce.Identity.API.Application.DTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;

        public string NickName { get; set; } = default!;
        public string AvatarUrl { get; set; } = default!;
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }

        public List<UserAddressDto> Addresses { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
    }
}
