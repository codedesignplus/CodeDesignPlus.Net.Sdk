using System;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.RabbitMQ.Services;

public class InitializeBackgroundService(IChannelProvider channelProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

        var domainEvents = types.Where(x => x.IsSubclassOf(typeof(DomainEvent)));

        foreach (var domainEvent in domainEvents)
        {
            channelProvider.ExchangeDeclare(domainEvent);
        }

        return Task.CompletedTask;
    }
}
