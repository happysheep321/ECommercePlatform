using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;

namespace Ecommerce.Identity.API.Application.Interfaces
{
    public interface IUserService
    {
        // 注册与登录
        Task<Guid> RegisterAsync(RegisterUserCommand command);
        Task<LoginResultDto> LoginAsync(LoginUserCommand command);

        // 忘记密码
        Task ResetPasswordByEmailAsync(ForgotPasswordCommand command);

        // 用户资料
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateUserProfileCommand command);

        Task ChangePasswordAsync(Guid userId, ChangePasswordCommand command);

        // 地址管理
        Task AddAddressAsync(Guid userId, AddUserAddressCommand command);
        Task UpdateAddressAsync(Guid userId, UpdateUserAddressCommand command);
        Task RemoveAddressAsync(Guid userId, Guid addressId);

        // 角色管理
        Task AssignRoleAsync(Guid userId, Guid roleId);
        Task RemoveRoleAsync(Guid userId, Guid roleId);

        // 用户状态管理
        Task ActivateAsync(ActivateUserCommand command);
        Task BanAsync(BanUserCommand command);
        Task FreezeAsync(FreezeUserCommand command);
        Task DeleteAsync(DeleteUserCommand command);

        //// 通知订阅
        //Task SubscribeAsync(Guid userId);
        //Task UnsubscribeAsync(Guid userId);
    }
}
