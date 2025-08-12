using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("角色名称不能为空")
                .MaximumLength(32).WithMessage("角色名称不能超过32字符");

            RuleFor(x => x.Description)
                .MaximumLength(128).WithMessage("描述不能超过128字符");
        }
    }
}
