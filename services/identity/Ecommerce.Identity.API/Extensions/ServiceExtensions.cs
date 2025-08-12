using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Options;
using Ecommerce.Identity.API.Application.Services;
using Ecommerce.Identity.API.Application.Validators;
using Ecommerce.Identity.API.Domain.Repositories;
using Ecommerce.Identity.API.Infrastructure;
using Ecommerce.Identity.API.Infrastructure.Behaviors;
using Ecommerce.Identity.API.Infrastructure.Repositories;
using Ecommerce.SharedKernel.Events;
using ECommerce.BuildingBlocks.Authentication;
using ECommerce.BuildingBlocks.Redis;
using ECommerce.SharedKernel.Interfaces;
using ECommerce.SharedKernel.Events;
using ECommerce.BuildingBlocks.Extensions;
using ECommerce.BuildingBolcks.EFCore;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Identity.API.Domain.Events;

namespace Ecommerce.Identity.API.Extensions
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
            // ===================== 1. 通用微服务配置 =====================
            services.AddMicroserviceCommonServices(
                configuration: config,
                serviceName: "Identity",
                swaggerTitle: "Ecommerce.Identity.API",
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

            // ===================== 3. 数据库 =====================
            services.AddDbContextPool<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("UserDb")));

            // ===================== 4. Repository 层 =====================
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // ===================== 5. Domain Service 层 =====================
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
