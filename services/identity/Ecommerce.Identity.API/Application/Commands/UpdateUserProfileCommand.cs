namespace ECommerce.Identity.API.Application.Commands
{
    public class UpdateUserProfileCommand
    {
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string AvatarUrl { get; set; } = default!;
        public string NickName { get; set; } = default!;
        /// <summary>
        /// �Ա�0=�У�1=Ů��2=δ֪��
        /// </summary>
        public int Gender { get; set; }
    }
}
