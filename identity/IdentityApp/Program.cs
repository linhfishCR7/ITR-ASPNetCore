// using Microsoft.EntityFrameworkCore;
// using IdentityApp.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages();
// builder.Services.AddDbContext<ProductDbContext>(opts =>
// {
//     opts.UseSqlServer(
//     builder.Configuration["ConnectionStrings:AppDataConnection"]);
// });
// builder.Services.AddHttpsRedirection(opts =>
// {
//     opts.HttpsPort = 44350;
// });
// builder.Services.AddDbContext<IdentityDbContext>(opts =>
// {
//     opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"],
//     opts => opts.MigrationsAssembly("IdentityApp")
//     );
// });
// builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<IdentityDbContext>();

// var app = builder.Build();

// // app.MapGet("/", () => "Hello World!");
// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.MapDefaultControllerRoute();
// app.UseAuthentication();
// app.UseAuthorization();

// app.Run();
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}