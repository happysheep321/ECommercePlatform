using MediatR;

namespace ECommerce.Identity.API.Application.Commands
{
    public class EnableRoleCommand : IRequest
    {
        public Guid RoleId { get; set; }
    }
}
