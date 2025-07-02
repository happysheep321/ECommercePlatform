using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Ecommerce.Identity.API.Domain.ValueObjects;
using ECommerce.BuildingBolcks.Authentication;
using ECommerce.BuildingBolcks.Redis;
using ECommerce.SharedKernel.Enums;
using ECommerce.SharedKernel.Interfaces;

namespace Ecommerce.Identity.API.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUserAddressRepository userAddressRepository;
        private readonly IUserLoginLogRepository userLoginLogRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly JwtTokenGenerator jwtTokenGenerator;
        private readonly IRedisHelper redisHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IUserRepository userRepository,IUserAddressRepository userAddressRepository , IUserLoginLogRepository userLoginLogRepository , IRoleRepository roleRepository, IPasswordHasher passwordHasher, JwtTokenGenerator jwtTokenGenerator,IRedisHelper redisHelper,IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.userAddressRepository = userAddressRepository;
            this.userLoginLogRepository = userLoginLogRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = passwordHasher;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.redisHelper = redisHelper;
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> RegisterAsync(RegisterUserCommand command)
        {
            if (await userRepository.ExistsByPhoneAsync(command.Phone))
                throw new InvalidOperationException("手机号已注册");

            var user = new User(
                Guid.NewGuid(),
                command.UserName,
                passwordHasher.HashPassword(command.Password),
                command.Email,
                command.Phone,
                command.UserType
            );

            string defaultRole = command.UserType switch
            {
                UserType.Normal => "Buyer",
                UserType.Seller => "Seller",
                UserType.Admin => "Admin",
                _ => throw new InvalidOperationException("未知的用户类型")
            };

            var role = await roleRepository.GetByNameAsync(defaultRole);
            if (role != null)
                user.AddRole(role);

            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            await redisHelper.DeleteAsync($"verify:code:register:{command.Phone}");
            return user.Id;
        }

        public async Task<LoginResultDto> LoginAsync(LoginUserCommand command)
        {
            var user = await userRepository.GetByUserNameAsync(command.UserName);
            if (user == null || !passwordHasher.VerifyPassword(user.PasswordHash!, command.Password))
            {
                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            var token = jwtTokenGenerator.GenerateToken(user.Id, user.UserName!, user.Type, user.Email!, user.PhoneNumber!);
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var device = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown";

            var loginLog = new UserLoginLog(user.Id, ip, device, "未知地区");
            await userLoginLogRepository.AddAsync(loginLog);
            await unitOfWork.SaveChangesAsync();

            return new LoginResultDto
            {
                Token=token.Token,
                Expiration= token.Expiration,
                UserName = user.UserName,
                AvatarUrl=user.Profile.AvatarUrl
            };
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("用户不存在");
            }
            return new UserProfileDto
            {
                UserId = userId,
                UserName = user.UserName!,
                Email = user.Email,
                Phone = user.PhoneNumber!,
                NickName = user.Profile.NickName,
                AvatarUrl = user.Profile.AvatarUrl,
                Birthday = user.Profile.Birthday,
                Gender = user.Profile.Gender,
                Addresses = user.Addresses.Select(addr => new UserAddressDto
                {
                    AddressId = addr.Id,
                    ReceiverName = addr.ReceiverName,
                    Phone = addr.Phone,
                    Region = addr.Region!,
                    Detail = addr.Detail,
                    IsDefault = addr.IsDefault,
                }).ToList(),
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    RoleId = ur.RoleId,
                    Name = ur.Role.Name!,
                    Description = ur.Role.Description,
                }).ToList(),
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateUserProfileCommand command)
        {
            var user=await userRepository.GetByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException("用户不存在");

            if (!string.IsNullOrWhiteSpace(command.Email))
            {
                user.Email = command.Email;
            }

            if (!string.IsNullOrWhiteSpace(command.Phone))
            {
                user.PhoneNumber = command.Phone;
            }

            var currentProfile = user.Profile;

            string updatedNickName = string.IsNullOrWhiteSpace(command.NickName) ? currentProfile!.NickName : command.NickName.Trim();
            string updatedAvatarUrl = string.IsNullOrWhiteSpace(command.AvatarUrl) ? currentProfile!.AvatarUrl : command.AvatarUrl;
            Gender updatedGender = (Gender)command.Gender;

            var updateProfile = new UserProfile(
                updatedNickName,
                updatedAvatarUrl,
                currentProfile.Birthday,
                updatedGender
                );

            user.UpdateProfile( updateProfile );
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddAddressAsync(Guid userId, AddUserAddressCommand command)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            var newAddress = new UserAddress(
                Guid.NewGuid(),
                userId,
                command.ReceiverName,
                command.Phone,
                command.Region,
                command.Detail,
                command.IsDefault
            );

            user.AddAddress(newAddress);
            await userAddressRepository.AddAsync(newAddress);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(Guid userId, UpdateUserAddressCommand command)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            // 先从仓储查找原地址
            var existAddress = await userAddressRepository.GetByIdAsync(command.AddressId);
            if (existAddress == null || existAddress.UserId != userId)
                throw new InvalidOperationException("地址不存在或不属于该用户");

            // 利用聚合根方法更新地址
            var updatedAddress = new UserAddress(
                existAddress.Id,
                userId,
                command.ReceiverName,
                command.Phone,
                command.Region,
                command.Detail,
                command.IsDefault
            );

            user.UpdateAddress(updatedAddress);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAddressAsync(Guid userId, Guid addressId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            var addressToRemove = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (addressToRemove == null)
                throw new InvalidOperationException("地址不存在");

            user.RemoveAddress(addressToRemove);
            userAddressRepository.Remove(addressToRemove);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            // 需要从角色仓储获取 Role 实体，假设你有角色仓储
            var role = await roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException("角色不存在");

            user.AddRole(role);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(Guid userId, Guid roleId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            user.RemoveRole(roleId);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddLoginLogAsync(Guid userId, string ip, string device, string location)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            var loginLog = new UserLoginLog(userId, ip, device, location);
            await userLoginLogRepository.AddAsync(loginLog);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
