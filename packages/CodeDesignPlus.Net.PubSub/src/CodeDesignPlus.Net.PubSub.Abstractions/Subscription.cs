namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    /// <summary>
    /// Information related to the event being registered in SubscriptionManager
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string EventName { get => this.EventType.Name; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the event.
        /// </summary>
        public Type EventType { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the event handler.
        /// </summary>
        public Type EventHandlerType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="Type"/> of the event.</param>
        /// <param name="eventHandlerType">The <see cref="Type"/> of the event handler.</param>
        private Subscription(Type eventType, Type eventHandlerType)
        {
            this.EventType = eventType;
            this.EventHandlerType = eventHandlerType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Subscription"/> with the specified event and event handler types.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
        /// <returns>Returns the event subscription information.</returns>
        public static Subscription Create<TEvent, TEventHandler>()
            where TEvent : IDomainEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            return new Subscription(typeof(TEvent), typeof(TEventHandler));
        }
    }
}