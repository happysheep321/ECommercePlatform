using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetUserByIdCommand : IRequest<UserProfileDto?>
    {
        public Guid UserId { get; set; }
    }
}
