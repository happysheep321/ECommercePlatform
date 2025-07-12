using ECommerce.BuildingBlocks.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SerilogConfiguration.ConfigureSerilog(builder.Configuration, "Background");
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ����ʱ��ӡ������
Log.Information("----------���� Background ΢����----------");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
