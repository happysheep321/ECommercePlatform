using MediatR;
using ECommerce.Identity.API.Application.DTOs;
using ECommerce.Identity.API.Application.Interfaces;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, List<UserListDto>>
    {
        private readonly IUserService userService;

        public GetAllUsersCommandHandler(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<List<UserListDto>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            return await userService.GetAllUsersAsync();
        }
    }
}
