using Ecommerce.Identity.API.Application.Commands;
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
            // 其他属性如 PhoneNumber、Profile 可通过方法或属性赋值
            await userRepository.AddAsync(user);
            return user.Id;
        }


    }
}
