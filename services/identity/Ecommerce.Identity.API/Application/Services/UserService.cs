using Ecommerce.Identity.API.Application.Interfaces;
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


    }
}
