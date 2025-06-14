using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Ecommerce.Identity.API.Infrastructure;
using ECommerce.BuildingBlocks.Logging;
using ECommerce.BuildingBolcks.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();

var connectionString = builder.Configuration.GetConnectionString("UserDb");
builder.Services.AddDbContextPool<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

SerilogConfiguration.ConfigureSerilog(builder.Configuration, "Identity");
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 启动时打印服务名
Log.Information("----------启动 Identity 微服务----------");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
