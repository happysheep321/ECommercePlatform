using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("角色名称不能为空")
                .MaximumLength(32).WithMessage("角色名称不能超过32字符");

            RuleFor(x => x.Description)
                .MaximumLength(128).WithMessage("描述不能超过128字符");
        }
    }
}
