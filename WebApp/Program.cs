using Microsoft.EntityFrameworkCore;
using WebApp.Models;
var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration[
    "ConnectionStrings:ProductConnection"]);
    opts.EnableSensitiveDataLogging(true);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.MapGet("/", () => "Hello World!");
app.UseMiddleware<WebApp.TestMiddleware>();
 
var context = app.Services.CreateScope().ServiceProvider
.GetRequiredService<DataContext>();
 
SeedData.SeedDatabase(context);
 
app.Run();