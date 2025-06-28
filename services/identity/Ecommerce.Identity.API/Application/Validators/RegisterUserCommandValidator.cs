using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("用户名不能为空")
                .MaximumLength(32).WithMessage("用户名不能超过32字符");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空")
                .MinimumLength(6).WithMessage("密码不能少于6位");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("手机号不能为空")
                .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式不正确");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("邮箱格式不正确");
        }
    }
}
