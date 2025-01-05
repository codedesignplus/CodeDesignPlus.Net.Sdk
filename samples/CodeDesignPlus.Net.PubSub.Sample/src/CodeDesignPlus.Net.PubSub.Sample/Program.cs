// See https://aka.ms/new-console-template for more information

using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Sample.Handlers;
using CodeDesignPlus.Net.PubSub.Sample.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();
builder.Services.AddPubSub(builder.Configuration);
builder.Services.AddSingleton<IMessage, InMemoryBroker>();

var host = builder.Build();

_ = Task.Run(() => host.Run());

await Task.Delay(5000);

var pubSub = host.Services.GetRequiredService<IPubSub>();

var userCreatedEvent = new UserCreatedEvent(Guid.NewGuid(), "John Doe");

await pubSub.PublishAsync(userCreatedEvent, CancellationToken.None);

Console.ReadLine();