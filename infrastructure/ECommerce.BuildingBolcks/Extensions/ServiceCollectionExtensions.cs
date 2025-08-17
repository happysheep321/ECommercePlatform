using ECommerce.BuildingBlocks.Authentication;
using ECommerce.BuildingBlocks.Redis;
using ECommerce.BuildingBlocks.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using System.Reflection;

namespace ECommerce.BuildingBlocks.Extensions
{
    /// <summary>
    /// 服务注册扩展方法集合
    /// 
    /// 提供微服务架构中常用的服务注册扩展方法，遵循DDD架构思想，
    /// 将基础设施服务与应用服务分离，提高代码复用性和可维护性。
    /// 
    /// 主要功能：
    /// - JWT认证服务配置
    /// - Swagger文档生成
    /// - Redis缓存服务
    /// - CORS跨域策略
    /// - 基础Web服务
    /// - 日志服务配置
    /// - MediatR中介者模式
    /// - FluentValidation数据验证
    /// - 微服务通用配置
    /// - 网关通用配置
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加JWT认证服务
        /// 
        /// 配置JWT Bearer Token认证，支持无状态身份验证。
        /// 适用于微服务架构中的分布式认证场景。
        /// 
        /// 配置要求：
        /// - appsettings.json中需要包含JwtSettings节点
        /// - 必须包含SecretKey、Issuer、Audience等配置项
        /// 
        /// 使用示例：
        /// <code>
        /// services.AddJwtAuthentication(configuration);
        /// </code>
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置对象</param>
        /// <returns>服务集合</returns>
        /// <exception cref="InvalidOperationException">当JwtSettings配置缺失时抛出</exception>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings configuration is missing");
            }

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
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
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services,
            string title,
            string version = "v1",
            string? xmlDocumentPath = null)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                
                // 添加XML注释支持
                if (!string.IsNullOrEmpty(xmlDocumentPath))
                {
                    // 使用指定的XML文档路径
                    if (File.Exists(xmlDocumentPath))
                    {
                        c.IncludeXmlComments(xmlDocumentPath);
                    }
                }
                else
                {
                    // 尝试自动查找XML文档
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
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

            return services;
        }

        public static IServiceCollection AddRedisServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new InvalidOperationException("Redis connection string is missing");
            }

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });

            services.AddScoped<IRedisHelper, RedisHelper>();

            return services;
        }

        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            string policyName = "AllowAll")
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddBaseWebServices(
            this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddSerilogServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string serviceName)
        {
            SerilogConfiguration.ConfigureSerilog(configuration, serviceName);
            return services;
        }

        public static IServiceCollection AddMediatRServices(
            this IServiceCollection services,
            Assembly? assembly = null)
        {
            services.AddMediatR(cfg =>
            {
                if (assembly != null)
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                }
                else
                {
                    // 默认注册当前程序集
                    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                }
            });

            return services;
        }

        public static IServiceCollection AddFluentValidationServices(
            this IServiceCollection services,
            params Type[] validatorMarkerTypes)
        {
            foreach (var markerType in validatorMarkerTypes)
            {
                services.AddValidatorsFromAssemblyContaining(markerType);
            }

            return services;
        }

        public static IServiceCollection AddMicroserviceCommonServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string serviceName,
            string swaggerTitle,
            bool enableJwtAuth = false,
            bool enableRedis = false,
            Assembly? mediatRAssembly = null,
            Assembly? validatorAssembly = null)
        {
            services.AddBaseWebServices();
            services.AddSerilogServices(configuration, serviceName);
            
            // 配置Swagger文档，包含XML注释支持
            // 使用serviceName参数来查找XML文档文件
            var xmlDocumentPath = Path.Combine(AppContext.BaseDirectory, $"{serviceName}.xml");
            services.AddSwaggerDocumentation(swaggerTitle, xmlDocumentPath: xmlDocumentPath);

            if (enableJwtAuth)
            {
                services.AddJwtAuthentication(configuration);
            }

            if (enableRedis)
            {
                services.AddRedisServices(configuration);
            }

            if (mediatRAssembly != null)
            {
                services.AddMediatRServices(mediatRAssembly);
            }

            if (validatorAssembly != null)
            {
                services.AddValidatorsFromAssembly(validatorAssembly);
            }

            return services;
        }

        public static IServiceCollection AddGatewayCommonServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddCorsPolicy();
            services.AddSerilogServices(configuration, "Gateway");
            services.AddReverseProxy()
                   .LoadFromConfig(configuration.GetSection("ReverseProxy"));

            return services;
        }
    }
}
