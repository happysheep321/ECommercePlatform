using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class UpdateUserProfileCommand : IRequest
    {
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string AvatarUrl { get; set; } = default!;
        public string NickName { get; set; } = default!;
        /// <summary>
        /// 性别：0=男，1=女，2=未知
        /// </summary>
        public int Gender { get; set; }
    }
}
