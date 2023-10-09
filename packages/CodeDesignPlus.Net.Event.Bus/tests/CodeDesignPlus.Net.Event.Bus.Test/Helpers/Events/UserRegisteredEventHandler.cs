using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events
{
    /// <summary>
    /// Event Handler process to type <see cref="UserRegisteredEvent"/>
    /// </summary>
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        /// <summary>
        /// Store events <see cref="Bus.Internal.Queue.QueueService{TEventHandler, TEvent}"/>
        /// </summary>
        public ConcurrentDictionary<Guid, UserRegisteredEvent> Events = new();

        /// <summary>
        /// Invoked by the event bus when an event to which it is subscribed is detected
        /// </summary>
        /// <param name="data">Event Info</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns>System.Threading.Tasks.Task represents an asynchronous operation.</returns>
        public Task HandleAsync(UserRegisteredEvent data, CancellationToken token)
        {
            Events.TryAdd(data.IdEvent, data);

            return Task.CompletedTask;
        }
    }
}
