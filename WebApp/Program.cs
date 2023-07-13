using Microsoft.EntityFrameworkCore;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration[
    "ConnectionStrings:ProductConnection"]);
    options.EnableSensitiveDataLogging(true);
});

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.MapControllers();
app.MapDefaultControllerRoute();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();
