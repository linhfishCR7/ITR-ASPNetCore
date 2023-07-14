
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using UploadS3.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration[
    "ConnectionStrings:S3Connection"]);
    options.EnableSensitiveDataLogging(true);
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AWS services
var awsOptions = builder.Configuration.GetAWSOptions();
awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(
    builder.Configuration["AWS:AccessKey"], builder.Configuration["AWS:SecretKey"]); // Set the access key and secret key
awsOptions.Region = Amazon.RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]); // Set the region
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

var app = builder.Build();

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
