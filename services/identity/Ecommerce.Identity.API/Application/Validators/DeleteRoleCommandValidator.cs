using ECommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace ECommerce.Identity.API.Application.Validators
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
