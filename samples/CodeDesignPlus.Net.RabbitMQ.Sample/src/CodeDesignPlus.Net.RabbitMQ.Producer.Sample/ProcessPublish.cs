using System;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.RabbitMQ.Producer.Sample;

public class ProcessPublish(IPubSub pubSub) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var userCreatedDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com");

        await pubSub.PublishAsync(userCreatedDomainEvent, CancellationToken.None);

        Console.WriteLine("Message published successfully");
    }
}
