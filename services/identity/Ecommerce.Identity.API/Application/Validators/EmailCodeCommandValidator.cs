using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class EmailCodeCommandValidator : AbstractValidator<EmailCodeCommand>
    {
        public EmailCodeCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("邮箱不能为空")
                .EmailAddress().WithMessage("邮箱格式不正确");
        }
    }
}
