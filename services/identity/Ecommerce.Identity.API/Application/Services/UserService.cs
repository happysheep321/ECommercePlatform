using Ecommerce.Identity.API.Application.Commands;
using Ecommerce.Identity.API.Application.DTOs;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Domain.Aggregates.UserAggregate;
using Ecommerce.Identity.API.Domain.Repositories;
using Ecommerce.Identity.API.Domain.ValueObjects;
using ECommerce.BuildingBlocks.Authentication;
using ECommerce.BuildingBlocks.Redis;
using ECommerce.SharedKernel.Enums;
using ECommerce.SharedKernel.Interfaces;
using Serilog;

namespace Ecommerce.Identity.API.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly JwtTokenGenerator jwtTokenGenerator;
        private readonly IRedisHelper redisHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UserService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IVerificationCodeService verificationCodeService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher, JwtTokenGenerator jwtTokenGenerator,IRedisHelper redisHelper,IUnitOfWork unitOfWork, ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor, IVerificationCodeService verificationCodeService)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = passwordHasher;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.redisHelper = redisHelper;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this.verificationCodeService = verificationCodeService;
        }

        public async Task<Guid> RegisterAsync(RegisterUserCommand command)
        {
            if (await userRepository.ExistsByEmailAsync(command.Email))
                throw new InvalidOperationException("邮箱已注册");

            var isVerified = await verificationCodeService.VerifyCodeAsync(command.Email, command.EmailVerifyCode);
            if (isVerified == false)
            {
                throw new InvalidOperationException("验证码错误或已过期");
            }

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
            else
                throw new InvalidOperationException($"默认角色 {defaultRole} 不存在，请检查系统角色初始化");

            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            return user.Id;
        }

        public async Task<LoginResultDto> LoginAsync(LoginUserCommand command)
        {
            var user = await userRepository.GetByUserNameAsync(command.UserName);
            if (user == null || !passwordHasher.VerifyPassword(user.PasswordHash!, command.Password))
            {
                logger.LogWarning("登录失败，用户名或密码错误。Username: {UserName}, SourceIp: {IP}",
                    command.UserName,
                    httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown");

                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            var token = jwtTokenGenerator.GenerateToken(user.Id, user.UserName!, user.Type, user.Email!, user.PhoneNumber!);
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var device = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown";

            logger.LogInformation("用户登录成功。UserId: {UserId}, UserName: {UserName}, IP: {IP}, Device: {Device}",
                user.Id, user.UserName, ip, device);

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            return new LoginResultDto
            {
                Token=token.Token,
                Expiration= token.Expiration,
                UserName = user.UserName,
                AvatarUrl=user.Profile.AvatarUrl
            };
        }

        public async Task ResetPasswordByEmailAsync(ForgotPasswordCommand command)
        {
            var user = await userRepository.GetByEmailAsync(command.Email);
            if (user == null)
                throw new InvalidOperationException("该邮箱未注册");

            var isVerified = await verificationCodeService.VerifyCodeAsync(command.Email, command.EmailVerifyCode);
            if (!isVerified)
                throw new InvalidOperationException("验证码无效或已过期");

            user.PasswordHash = passwordHasher.HashPassword(command.NewPassword);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("用户不存在");
            }

            var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            var roles = await roleRepository.GetByIdsAsync(roleIds);
            var roleMap = roles.ToDictionary(r => r.Id, r => r);

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
                Roles = user.UserRoles.Select(ur => 
                {
                    var role = roleMap[ur.RoleId];

                    return new RoleDto
                    {
                        RoleId = role.Id,
                        Name = role.Name ?? string.Empty,
                        Description = role.Description ?? string.Empty,
                        IsEnabled = role.Enabled,
                        IsSystem = role.IsSystemRole
                    };
                }).ToList()
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

            user.UpdateProfile(updateProfile);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid userId,ChangePasswordCommand command)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            var isCorrect = passwordHasher.VerifyPassword(user.PasswordHash, command.CurrentPassword);
            if (!isCorrect)
                throw new InvalidOperationException("当前密码不正确");

            var newPasswordHash = passwordHasher.HashPassword(command.NewPassword);
            user.PasswordHash = newPasswordHash;

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddAddressAsync(Guid userId, AddUserAddressCommand command)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            var newAddress = new UserAddress(
                userId,
                command.ReceiverName,
                command.Phone,
                command.Region,
                command.Detail,
                command.IsDefault
            );

            user.AddAddress(newAddress);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(Guid userId, UpdateUserAddressCommand command)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("用户不存在");

            // 先从仓储查找原地址
            var existAddress = user.Addresses.FirstOrDefault(a => a.Id == command.AddressId);
            if (existAddress == null)
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
            userRepository.Update(user);
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
            userRepository.Update(user);
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

            var role = await roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException("角色不存在");

            if (user.UserRoles.Count == 1)
                throw new InvalidOperationException("用户不能没有角色");

            user.RemoveRole(roleId);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
