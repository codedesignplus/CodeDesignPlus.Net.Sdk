using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    public interface IPubSub
    {
        /// <summary>
        /// Publish a domain event
        /// </summary>
        /// <param name="event">Domain event to publish</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task that represents the asynchronous operation</returns>
        Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken);
        /// <summary>
        /// Publish a list of domain events
        /// </summary>
        /// <param name="event">Domains event to publish</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task that represents the asynchronous operation</returns>
        Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken);
    }
}
