
using CodeDesignPlus.Net.RabbitMQ.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.RabbitMQ.Producer.Sample;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddRabbitMQ<Program>(builder.Configuration);

builder.Services.AddHostedService<ProcessPublish>();

var host = builder.Build();

host.Run();
