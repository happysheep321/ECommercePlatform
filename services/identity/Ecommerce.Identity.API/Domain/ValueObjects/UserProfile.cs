using ECommerce.SharedKernel.Base;
using ECommerce.SharedKernel.Enums;

namespace ECommerce.Identity.API.Domain.ValueObjects
{
    public class UserProfile:ValueObject
    {
        private const string DefaultAvatarUrl = "/images/default-avatar.png"; // 默认用户头像

        public string NickName { get;private set; }=string.Empty;

        public string AvatarUrl { get; private set; } = string.Empty;

        public DateTime Birthday { get; private set; }

        public Gender Gender { get; private set; }=Gender.Unspecified;

        protected UserProfile() { }

        public UserProfile(string nickName, string avatarUrl, DateTime birthday, Gender gender)
        {
            NickName = nickName;
            AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? DefaultAvatarUrl : avatarUrl;
            Birthday = birthday;
            Gender = gender;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NickName;
            yield return AvatarUrl;
            yield return Birthday;
            yield return Gender;
        }
    }
}
