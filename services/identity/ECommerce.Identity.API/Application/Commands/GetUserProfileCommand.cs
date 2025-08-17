using MediatR;
using ECommerce.Identity.API.Application.DTOs;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GetUserProfileCommand : IRequest<UserProfileDto>
{
    // 移除UserId字段，从JWT Token中获取
}
}
