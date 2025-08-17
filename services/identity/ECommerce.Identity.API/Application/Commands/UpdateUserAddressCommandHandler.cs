using MediatR;
using ECommerce.Identity.API.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Identity.API.Application.Commands
{
    public class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand>
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UpdateUserAddressCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                ?? httpContextAccessor.HttpContext?.User.FindFirst("sub");
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("用户未认证");
            }

            await userService.UpdateAddressAsync(userId, request);
        }
    }
}
