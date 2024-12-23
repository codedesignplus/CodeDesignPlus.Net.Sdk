using CodeDesignPlus.Net.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddObservability(builder.Configuration, builder.Environment);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();