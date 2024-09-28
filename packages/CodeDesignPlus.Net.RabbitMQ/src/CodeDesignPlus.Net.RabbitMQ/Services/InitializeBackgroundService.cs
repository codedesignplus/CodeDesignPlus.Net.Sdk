using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Background service to initialize RabbitMQ exchanges for domain events.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InitializeBackgroundService"/> class.
/// </remarks>
/// <param name="channelProvider">The channel provider to declare exchanges.</param>
public class InitializeBackgroundService(IChannelProvider channelProvider) : BackgroundService
{
    private readonly IChannelProvider channelProvider = channelProvider;

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts. It initializes the RabbitMQ exchanges for all domain events.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the host is performing a graceful shutdown.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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