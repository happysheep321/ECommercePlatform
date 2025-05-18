using ECommerce.BuildingBlocks.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SerilogConfiguration.ConfigureSerilog(builder.Configuration, "Cart");
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ����ʱ��ӡ������
Log.Information("----------���� Cart ΢����----------");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
