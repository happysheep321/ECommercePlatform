namespace Ecommerce.Identity.API.Application.Commands
{
    public class UpdateUserProfileCommand
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? NickName { get; set; }
        /// <summary>
        /// �Ա�0=�У�1=Ů��2=δ֪��
        /// </summary>
        public int? Gender { get; set; }
    }
}
