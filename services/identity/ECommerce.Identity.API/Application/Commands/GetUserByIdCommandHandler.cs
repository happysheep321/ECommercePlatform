using MediatR;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, UserProfileDto?>
    {
        private readonly IUserService userService;

        public GetUserByIdCommandHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<UserProfileDto?> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
        {
            return await userService.GetUserByIdAsync(request.UserId);
        }
    }
}
