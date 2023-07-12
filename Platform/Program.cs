
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapFallback(async context => {
    await context.Response
    .WriteAsync($"HTTPS Request: {context.Request.IsHttps} \n");
    await context.Response.WriteAsync("Hello World!");
});
app.MapGet("/", () => "Hello World!");


app.Run();
