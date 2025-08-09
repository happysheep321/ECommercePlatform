using Ecommerce.Identity.API.Application.Commands;
using FluentValidation;

namespace Ecommerce.Identity.API.Application.Validators
{
    public class EnableRoleCommandValidator : AbstractValidator<EnableRoleCommand>
    {
        public EnableRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
