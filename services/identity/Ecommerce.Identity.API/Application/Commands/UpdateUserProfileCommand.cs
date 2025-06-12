namespace Ecommerce.Identity.API.Application.Commands
{
    public class UpdateUserProfileCommand
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? NickName { get; set; }
        /// <summary>
        /// 性别（0=男，1=女，2=未知）
        /// </summary>
        public int? Gender { get; set; }
    }
}
