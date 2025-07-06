using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("当前密码不能为空");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("新密码不能为空")
                .MinimumLength(6).WithMessage("密码长度不能少于6位");
        }
    }
}
