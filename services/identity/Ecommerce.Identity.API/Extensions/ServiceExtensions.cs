using ECommerce.BuildingBlocks.Logging;
using ECommerce.BuildingBolcks.Authentication;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Ecommerce.Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.Identity.API.Domain.Repositories;
using Ecommerce.Identity.API.Infrastructure.Repositories;
using ECommerce.SharedKernel.Interfaces;
using MediatR;
using Ecommerce.Identity.API.Infrastructure.Behaviors;
using Ecommerce.Identity.API.Application.Validators;
using ECommerce.BuildingBolcks.Redis;
using StackExchange.Redis;

namespace Ecommerce.Identity.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentityServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;
            var env = builder.Environment;

            services.AddInfrastructureServices();
            services.AddRepositories();
            services.AddDomainServices();
            services.AddMediatRAndBehaviors();

            services.AddDatabase(config);
            services.AddJwtAuthentication(config);
            services.AddCustomLogging(config, "Identity");

            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerSecurity();
        }

        private static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(configuration!);
            });
            services.AddScoped<IRedisHelper, RedisHelper>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserLoginLogRepository, UserLoginLogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
        }

        private static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }

        private static void AddMediatRAndBehaviors(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<IUserService>();
            });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<AddUserAddressCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserAddressCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserProfileCommandValidator>();
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("UserDb");
            services.AddDbContextPool<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata=false; // 开发环境可以关闭 HTTPS 要求
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew=TimeSpan.Zero // 关闭默认的时间偏移，避免令牌过期问题
                };
            });
        }

        private static void AddSwaggerSecurity(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Ecommerce.Identity.API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "输入格式: Bearer {token}",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                          new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>() // 这里可以指定需要的作用域，如果没有则传空数组
                    }
                });
            });
        }

        private static void AddCustomLogging(this IServiceCollection services, IConfiguration config, string serviceName)
        {
            SerilogConfiguration.ConfigureSerilog(config, serviceName);
        }
    }
}
