using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.Identity.API.Application.Options;
using ECommerce.Identity.API.Application.Services;
using ECommerce.Identity.API.Application.Validators;
using ECommerce.Identity.API.Domain.Repositories;
using ECommerce.Identity.API.Infrastructure;
using ECommerce.Identity.API.Infrastructure.Behaviors;
using ECommerce.Identity.API.Infrastructure.Repositories;
using ECommerce.SharedKernel.Events;
using ECommerce.BuildingBlocks.Authentication;
using ECommerce.SharedKernel.Interfaces;
using ECommerce.BuildingBlocks.Extensions;
using ECommerce.BuildingBolcks.EFCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ECommerce.Identity.API.Domain.Events;

namespace ECommerce.Identity.API.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// 注册 Identity 微服务的全部模块
        /// </summary>
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            // ===================== 1. 通用微服务配�?=====================
            services.AddMicroserviceCommonServices(
                configuration: config,
                serviceName: "ECommerce.Identity.API",
                swaggerTitle: "ECommerce.Identity.API",
                enableJwtAuth: true,
                enableRedis: true,
                mediatRAssemblies: new[]
                {
                    typeof(IUserService),
                    typeof(UserRoleAssignedEvent),
                    typeof(UserRoleRemovedEvent),
                    typeof(RolePermissionGrantedEvent),
                    typeof(RolePermissionRevokedEvent)
                },
                validatorAssemblies: new[]
                {
                    typeof(RegisterUserCommandValidator),
                    typeof(LoginUserCommandValidator),
                    typeof(AddUserAddressCommandValidator),
                    typeof(UpdateUserAddressCommandValidator),
                    typeof(UpdateUserProfileCommandValidator)
                }
            );

            // ===================== 2. Identity 特定服务 =====================
            services.AddSingleton<JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===================== 3. 数据�?=====================
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("UserDb")));

            // ===================== 4. Repository �?=====================
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // ===================== 5. Domain Service �?=====================
            services.Configure<EmailOptions>(config.GetSection("Email"));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IVerificationCodeService, EmailVerificationService>();

            // ===================== 6. Pipeline 行为 =====================
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // ===================== 7. Domain Event Dispatcher =====================
            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

            // ===================== 8. EF Core 迁移服务 =====================
            services.AddEFCoreMigrationService();

            return services;
        }
    }
}
