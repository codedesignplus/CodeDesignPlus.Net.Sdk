using CodeDesignPlus.Net.Kafka.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder.Configuration);

var host = builder.Build();
host.Run();
