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
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using Ecommerce.Identity.API.Domain.Events;

namespace Ecommerce.Identity.API.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// ע�� Identity ΢���������ȫ��ģ��
        /// </summary>
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            // ===================== 1. ������ʩ & ���� =====================
            services.AddSingleton<JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfig = config.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(redisConfig!);
            });
            services.AddScoped<IRedisHelper, RedisHelper>();

            // ===================== 2. ���ݿ� =====================
            services.AddDbContextPool<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("UserDb")));

            // ===================== 3. Repository �� =====================
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // ===================== 4. Domain Service �� =====================
            services.Configure<EmailOptions>(config.GetSection("Email"));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IVerificationCodeService, EmailVerificationService>();

            // ===================== 5. Application �� =====================
            services.AddMediatR(cfg =>
            {
                // �ϲ� Application �� Domain Event �ĳ���ע��
                cfg.RegisterServicesFromAssemblies(
                    typeof(IUserService).Assembly,
                    typeof(UserRoleAssignedEvent).Assembly,
                    typeof(UserRoleRemovedEvent).Assembly,
                    typeof(RolePermissionGrantedEvent).Assembly,
                    typeof(RolePermissionRevokedEvent).Assembly
                );
            });

            // Pipeline + ��֤��
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddUserAddressCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserAddressCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserProfileCommandValidator>();

            // ===================== 6. Domain Event Dispatcher =====================
            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

            // ===================== 7. JWT ��֤ =====================
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
            var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // ===================== 8. Swagger =====================
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce.Identity.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "�����ʽ: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ===================== 9. ���� Controller & ���� =====================
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
