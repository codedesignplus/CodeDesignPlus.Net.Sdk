using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.PubSub.Exceptions;

namespace CodeDesignPlus.Net.PubSub.Services
{
    /// <summary>
    /// Default implementation for the <see cref="ISubscriptionManager"/> service.
    /// </summary>
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<string, List<Subscription>> handlers = [];
        private readonly ILogger<SubscriptionManager> logger;

        /// <summary>
        /// Determines if there are any registered events.
        /// </summary>
        public bool Any() => this.handlers.Any();

        /// <summary>
        /// Event triggered when an event has been removed.
        /// </summary>
        public event EventHandler<Subscription> OnEventRemoved;

        /// <summary>
        /// Gets the name of the event for a given event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>The name of the event type.</returns>
        public string GetEventKey<TEvent>() where TEvent : IDomainEvent
        {
            var attribute = typeof(TEvent).GetCustomAttribute<KeyAttribute>();	

            if (attribute is null)	
                throw new KeyAttributeNotFoundException(typeof(TEvent).Name);

            return typeof(TEvent).GetCustomAttribute<KeyAttribute>().Key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to manage the logs.</param>
        /// <exception cref="ArgumentNullException">Thrown when the logger is null.</exception>
        public SubscriptionManager(ILogger<SubscriptionManager> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.logger.LogInformation("SubscriptionManager initialized.");
        }

        /// <summary>
        /// Adds a subscription to the Subscription Manager.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <typeparam name="TEventHandler">The event handler type.</typeparam>
        /// <exception cref="EventHandlerAlreadyRegisteredException{TEvent, TEventHandler}">Thrown when the event handler is already registered.</exception>
        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IDomainEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = this.GetEventKey<TEvent>();

            if (!this.HasSubscriptionsForEvent<TEvent>())
            {
                this.handlers.Add(eventName, []);
                this.logger.LogInformation("Event {eventName} added to handlers.", eventName);
            }

            if (this.handlers[eventName].Any(x => x.EventHandlerType == typeof(TEventHandler)))
            {
                this.logger.LogWarning("EventHandler {TEventHandler} for event {eventName} already registered.", typeof(TEventHandler).Name, eventName);
                throw new EventHandlerAlreadyRegisteredException<TEvent, TEventHandler>();
            }

            this.handlers[eventName].Add(Subscription.Create<TEvent, TEventHandler>());
            this.logger.LogInformation("EventHandler {TEventHandler} for event {eventName} registered.", typeof(TEventHandler).Name, eventName);
        }

        /// <summary>
        /// Removes a subscription from the Subscription Manager.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <typeparam name="TEventHandler">The event handler type.</typeparam>
        public void RemoveSubscription<TEvent, TEventHandler>()
           where TEvent : IDomainEvent
           where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();
            var suscription = this.FindSubscription<TEvent, TEventHandler>();

            this.handlers[eventName].Remove(suscription);
            this.logger.LogInformation("Removed subscription for EventHandler {TEventHandler} from event {eventName}.", typeof(TEventHandler).Name, eventName);

            if (!this.handlers[eventName].Any())
            {
                this.handlers.Remove(eventName);
                this.OnEventRemoved?.Invoke(this, suscription);
                this.logger.LogInformation("Event {eventName} has no subscriptions and has been removed from handlers.", eventName);
            }
        }

        /// <summary>
        /// Checks if there are registered event handlers for a specific integration event.
        /// </summary>
        /// <typeparam name="TEvent">Integration event to check if it has an associated event handler.</typeparam>
        /// <returns>Returns true if the integration event has an associated event handler.</returns>
        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IDomainEvent
        {
            var eventName = this.GetEventKey<TEvent>();

            return this.handlers.ContainsKey(eventName);
        }

        /// <summary>
        /// Retrieves the subscription details for an integration event.
        /// </summary>
        /// <typeparam name="TEvent">Integration event to query.</typeparam>
        /// <exception cref="EventIsNotRegisteredException">Thrown if the specified event is not registered.</exception>
        /// <returns>Returns the subscription details for an event.</returns>
        public IEnumerable<Subscription> GetHandlers<TEvent>() where TEvent : IDomainEvent
        {
            var eventName = this.GetEventKey<TEvent>();

            if (!handlers.TryGetValue(eventName, out List<Subscription> value))
                throw new EventIsNotRegisteredException();

            return value;
        }

        /// <summary>
        /// Searches and returns the subscription details based on the event's name and type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to search for.</typeparam>
        /// <typeparam name="TEventHandler">Type of the event handler to search for.</typeparam>
        /// <exception cref="EventIsNotRegisteredException">Thrown if trying to find a subscription for an unregistered event.</exception>
        /// <returns>Returns the subscription details for the provided event type. If the event is not found, returns null.</returns>
        public Subscription FindSubscription<TEvent, TEventHandler>()
             where TEvent : IDomainEvent
             where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = this.GetEventKey<TEvent>();

            if (!this.HasSubscriptionsForEvent<TEvent>())
            {
                this.logger.LogWarning("Attempted to find subscription for unregistered event {eventName}.", eventName);
                throw new EventIsNotRegisteredException();
            }

            return this.handlers[eventName].SingleOrDefault(s => s.EventHandlerType == typeof(TEventHandler));
        }

        /// <summary>
        /// Searches and returns all subscription details based on the event's name and type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to search for.</typeparam>
        /// <exception cref="EventIsNotRegisteredException">Thrown if the specified event is not registered.</exception>
        /// <returns>Returns the list of subscription details for the provided event type. If none are found, returns null.</returns>
        public List<Subscription> FindSubscriptions<TEvent>()
             where TEvent : IDomainEvent
        {
            var eventName = this.GetEventKey<TEvent>();

            if (!this.HasSubscriptionsForEvent<TEvent>())
                throw new EventIsNotRegisteredException();

            return this.handlers[eventName];
        }

        /// <summary>
        /// Clears all subscriptions from the manager.
        /// </summary>
        public void Clear()
        {
            this.handlers.Clear();
            this.logger.LogInformation("Subscription manager cleared all event handlers.");
        }
    }
}
