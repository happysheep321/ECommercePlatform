using Ecommerce.Identity.API.Infrastructure;
using ECommerce.BuildingBlocks.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// ����ʱ��ӡ������
Log.Information("----------���� Identity ΢����----------");

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
