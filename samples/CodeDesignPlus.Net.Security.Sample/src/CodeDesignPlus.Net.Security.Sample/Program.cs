using CodeDesignPlus.Net.Security.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSecurity(builder.Configuration);

var app = builder.Build();

app.UseAuth();

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.Run();
