using CodeDesignPlus.Net.ServiceBus.Extensions;
using CodeDesignPlus.Net.ServiceBus.Producer.Sample;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddServiceBus<Program>(builder.Configuration);
builder.Services.AddHostedService<ProcessPublish>();

var host = builder.Build();
host.Run();
