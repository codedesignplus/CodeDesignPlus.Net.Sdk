using CodeDesignPlus.Net.Redis.PubSub.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisPubSub(builder.Configuration);

var host = builder.Build();

host.Run();
