using MediatR;
using ECommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ECommerce.Identity.API.Application.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DeleteUserCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // 验证当前用户是否有管理员权限
            var currentUser = httpContextAccessor.HttpContext?.User;
            if (currentUser == null || !currentUser.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("需要管理员权限");
            }

            await userService.DeleteAsync(request);
        }
    }
}
