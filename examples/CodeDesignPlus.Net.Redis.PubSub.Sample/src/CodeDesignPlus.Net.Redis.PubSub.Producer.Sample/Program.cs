// See https://aka.ms/new-console-template for more information

using CodeDesignPlus.Net.Redis.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Redis.PubSub.Producer.Sample;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisPubSub(builder.Configuration);

var host = builder.Build();

_ = Task.Run(() => host.Run());

await Task.Delay(5000);

var producer = host.Services.GetRequiredService<IPubSub>();

var userCreatedDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com");

await producer.PublishAsync(userCreatedDomainEvent, CancellationToken.None);

Console.WriteLine("Message published successfully");

Console.ReadLine();