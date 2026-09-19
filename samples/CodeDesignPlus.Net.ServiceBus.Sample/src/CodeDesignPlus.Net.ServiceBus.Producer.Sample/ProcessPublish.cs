using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.ServiceBus.Producer.Sample;

public class ProcessPublish(IPubSub pubSub) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var userCreatedDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com");

        await pubSub.PublishAsync(userCreatedDomainEvent, stoppingToken);

        Console.WriteLine("Message published successfully");
    }
}
