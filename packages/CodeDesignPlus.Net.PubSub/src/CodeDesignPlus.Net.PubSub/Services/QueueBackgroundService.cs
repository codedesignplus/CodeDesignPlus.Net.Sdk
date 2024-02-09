using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.PubSub.Services
{
    /// <summary>
    /// Provides a background service to manage the queue for event handling.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class QueueBackgroundService<TEventHandler, TEvent> : BackgroundService
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        private readonly IQueueService<TEventHandler, TEvent> queueService;
        private readonly ILogger<QueueBackgroundService<TEventHandler, TEvent>> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBackgroundService{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="queueService">The queue service to manage the event handling.</param>
        /// <param name="logger">The logger to manage the logs.</param>
        public QueueBackgroundService(IQueueService<TEventHandler, TEvent> queueService, ILogger<QueueBackgroundService<TEventHandler, TEvent>> logger)
        {
            this.queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.logger.LogInformation("QueueBackgroundService for EventHandler: {TEventHandler} and Event: {TEvent} has been initialized.", typeof(TEventHandler).Name, typeof(TEvent).Name);
        }

        /// <summary>
        /// Executes the background service to manage the queue for event handling.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token used to propagate notifications that operations should be canceled.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Background service for event handling started.");

            stoppingToken.Register(() => logger.LogInformation("Background service for event handling is stopping."));

            this.queueService.DequeueAsync(stoppingToken);

            return Task.CompletedTask;
        }
    }
}
