using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    public interface IEventQueueService
    {
        /// <summary>
        /// Add an object to the end of the concurrent queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
        /// <param name="event">The object to add to the concurrent queue. The value can be a null reference (Nothing in Visual Basic) for reference types.</param>
        Task EnqueueAsync(IDomainEvent @event, CancellationToken cancellationToken);
        /// <summary>
        /// Try to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="token">The cancellation token that will be assigned to the new task.</param>
        /// <returns>Return a <see cref="Task{T}"/></returns>
        Task DequeueAsync(CancellationToken token);
    }
}
