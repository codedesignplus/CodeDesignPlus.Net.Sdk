using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Background service that initializes the RabbitMQ exchanges for all domain events.
/// </summary>
/// <typeparam name="TAssembly">The assembly where the domain events are located.</typeparam>
/// <param name="channelProvider">The provider of the RabbitMQ channels.</param>
/// <param name="logger">The logger instance.</param>
public class DeclareExchangeBackgroundService<TAssembly>(IChannelProvider channelProvider, ILogger<DeclareExchangeBackgroundService<TAssembly>> logger) : BackgroundService
{
    private readonly IChannelProvider channelProvider = channelProvider;

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts. It initializes the RabbitMQ exchanges for all domain events.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the host is performing a graceful shutdown.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var types = typeof(TAssembly).Assembly.GetTypes();

            var domainEvents = types.Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(DomainEvent)));

            foreach (var domainEvent in domainEvents)
            {
                await channelProvider.ExchangeDeclareAsync(domainEvent, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while declaring the exchanges.");

            throw;
        }
    }
}