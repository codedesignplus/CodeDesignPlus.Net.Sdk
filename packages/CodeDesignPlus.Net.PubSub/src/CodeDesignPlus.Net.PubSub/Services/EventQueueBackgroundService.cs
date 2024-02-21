using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.PubSub.Services
{
    /// <summary>
    /// Provides a background service to manage the queue for event handling.
    /// </summary>
    public class EventQueueBackgroundService : BackgroundService
    {
        private readonly IEventQueueService queueService;
        private readonly ILogger<EventQueueBackgroundService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBackgroundService{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="queueService">The queue service to manage the event handling.</param>
        /// <param name="logger">The logger to manage the logs.</param>
        public EventQueueBackgroundService(IEventQueueService queueService, ILogger<EventQueueBackgroundService> logger)
        {
            this.queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.logger.LogInformation("EventQueueBackgroundService has been initialized.");
        }

        /// <summary>
        /// Executes the background service to manage the queue for event handling.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token used to propagate notifications that operations should be canceled.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("EventQueueBackgroundService service for event handling started.");

            stoppingToken.Register(() => logger.LogInformation("EventQueueBackgroundService service for event handling is stopping."));

            this.queueService.DequeueAsync(stoppingToken);

            return Task.CompletedTask;
        }
    }
}
