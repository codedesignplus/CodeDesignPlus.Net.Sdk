using CodeDesignPlus.Net.RabbitMQ.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddRabbitMQ<Program>(builder.Configuration);

var host = builder.Build();
host.Run();
