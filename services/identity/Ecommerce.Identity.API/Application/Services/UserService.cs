using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using ECommerce.BuildingBolcks.Authentication;

namespace Ecommerce.Identity.API.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Guid> RegisterAsync(RegisterUserCommand command)
        {
            var user = new User(
                command.UserName,
                passwordHasher.HashPassword(command.Password),
                command.Email!
            );

            await userRepository.AddAsync(user);
            return user.Id;
        }

        public async Task<LoginResultDto> LoginAsync(LoginUserCommand command)
        {
            var user = await userRepository.GetByUserNameAsync(command.UserName);
            if (user == null || !passwordHasher.VerifyPassword(user.PasswordHash, command.Password))
            {
                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            var token 
            return new LoginResultDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsPhoneConfirmed = user.IsPhoneConfirmed
            };
        }

    }
}
