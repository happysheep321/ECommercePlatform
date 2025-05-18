using ECommerce.BuildingBlocks.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SerilogConfiguration.ConfigureSerilog(builder.Configuration, "Gateway");
builder.Host.UseSerilog();
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReversProxy"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 启动时打印服务名
Log.Information("----------启动 Gateway 服务----------");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();
