using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Event.Bus.Services
{
    /// <summary>
    /// Provides a background service to manage the queue for event handling.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class QueueBackgroundService<TEventHandler, TEvent> : BackgroundService, IEventBusBackgroundService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        private readonly IQueueService<TEventHandler, TEvent> queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBackgroundService{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="queueService">The queue service to manage the event handling.</param>
        public QueueBackgroundService(IQueueService<TEventHandler, TEvent> queueService)
        {
            this.queueService = queueService;
        }

        /// <summary>
        /// Executes the background service to manage the queue for event handling.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token used to propagate notifications that operations should be canceled.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.queueService.DequeueAsync(stoppingToken);
        }
    }
}
