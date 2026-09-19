using CodeDesignPlus.Net.ServiceBus.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddServiceBus<Program>(builder.Configuration);

var host = builder.Build();
host.Run();
