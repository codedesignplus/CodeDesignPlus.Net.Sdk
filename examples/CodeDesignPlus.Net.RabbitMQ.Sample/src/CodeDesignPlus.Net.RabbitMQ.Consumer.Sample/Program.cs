using CodeDesignPlus.Net.RabbitMQ.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRabbitMQ<Program>(builder.Configuration);

var host = builder.Build();
host.Run();
