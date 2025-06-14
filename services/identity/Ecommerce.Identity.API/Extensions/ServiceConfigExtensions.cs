using ECommerce.BuildingBlocks.Logging;
using ECommerce.BuildingBolcks.Authentication;
using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Ecommerce.Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Identity.API.Extensions
{
    public static class ServiceConfigExtensions
    {
        public static void ConfigureIdentityServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;
            var env = builder.Environment;

            services.AddApplicationServices();
            services.AddDatabase(config);
            services.AddJwtAuthentication(config);
            services.AddCustomLogging(config, "Identity");

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // 1. 应用服务（DI）
        private static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserService, UserService>();
        }

        // 2. 数据库
        private static void AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("UserDb");
            services.AddDbContextPool<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // 3. JWT 身份验证
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
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }

        // 4. Serilog 日志
        private static void AddCustomLogging(this IServiceCollection services, IConfiguration config, string serviceName)
        {
            SerilogConfiguration.ConfigureSerilog(config, serviceName);
        }
    }
}
