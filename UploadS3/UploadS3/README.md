### Introduction

## Follow this article 
https://codewithmukesh.com/blog/working-with-aws-s3-using-aspnet-core/

## Enable access public on item on s3
https://havecamerawilltravel.com/how-allow-public-access-amazon-bucket/

### Project Flow:

- Create account AWS

- Initial project And Install Package

`dotnet add package AWSSDK.Extensions.NETCore.Setup
dotnet add package AWSSDK.S3
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer`

- Setup appsettings.json

`"AWS": {
    // AWS
    "Region": "Region", // Get from AWS
    "AccessKey": "AccessKey", // Get from AWS
    "SecretKey": "SecretKey" // Get from AWS
  },
  "ConnectionStrings": {
    // SQL
    "S3Connection": "Server=(localdb)\\MSSQLLocalDB;Database=S3Db;MultipleActiveResultSets=true"
  }`

- Setup Progeam.cs

`   // Configure SQL
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration[
        "ConnectionStrings:S3Connection"]);
        options.EnableSensitiveDataLogging(true);
    });
    // Configure AWS services
    var awsOptions = builder.Configuration.GetAWSOptions();
    awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(
        builder.Configuration["AWS:AccessKey"], builder.Configuration["AWS:SecretKey"]); // Set the access key and secret key
    awsOptions.Region = Amazon.RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]); // Set the region
    builder.Services.AddDefaultAWSOptions(awsOptions);
    builder.Services.AddAWSService<IAmazonS3>();
`
- Create model
- Create Controller
- Run