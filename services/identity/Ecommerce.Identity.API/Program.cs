using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Application.Services;
using Ecommerce.Identity.API.Extensions;
using Ecommerce.Identity.API.Infrastructure;
using ECommerce.BuildingBlocks.Logging;
using ECommerce.BuildingBolcks.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureIdentityServices();

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
