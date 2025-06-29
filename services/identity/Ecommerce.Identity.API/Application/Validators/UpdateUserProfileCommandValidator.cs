using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("邮箱格式不正确");

            RuleFor(x => x.Phone)
                .Matches(@"^1[3-9]\d{9}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("手机号格式不正确");

            RuleFor(x => x.AvatarUrl)
                .Must(BeAValidUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl))
                .WithMessage("头像地址格式不合法");

            RuleFor(x => x.NickName)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.NickName))
                .WithMessage("昵称不能超过20个字符");

            RuleFor(x => x.Gender)
                .InclusiveBetween(0, 2)
                .WithMessage("性别只能是 0（男）、1（女）或 2（未知）");
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
