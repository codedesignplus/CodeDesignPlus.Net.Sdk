using System.Linq.Expressions;
using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

/// <summary>
/// Represents the contract to be implemented by the aggregate root.
/// </summary>
public abstract class AggregateRoot : Core.Abstractions.AggregateRoot, IAggregateRoot
{
    /// <summary>
    /// The cache of the delegates to create instances of the aggregate root.
    /// </summary>
    private static readonly Dictionary<Type, Delegate> instanceDelegatesCache = [];
    /// <summary>
    /// The cache of the methods to apply the changes that occur in the aggregate root.
    /// </summary>
    private static readonly Dictionary<Type, MethodInfo> applyMethodsCache = [];

    /// <summary>
    /// The category of the aggregate root.
    /// </summary>
    public abstract string Category { get; protected set; }

    /// <summary>
    /// The version of the aggregate root.
    /// </summary>
    public long Version { get; private set; } = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot(Guid id) : base(id) { }

    /// <summary>
    /// Add a domain event to the list of events that have occurred in the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to add to the list of events that have occurred in the aggregate root.</param>
    public override void AddEvent(IDomainEvent @event)
    {
        @event.Metadata.Add("Version", ++this.Version);
        @event.Metadata.Add("Category", this.Category);
        @event.Metadata.Add("OccurredAt", @event.OccurredAt);
        @event.Metadata.Add("EventId", @event.EventId);
        @event.Metadata.Add("EventType", @event.EventType);

        base.AddEvent(@event);

        ApplyEvent(@event);
    }

    /// <summary>
    /// Apply the changes that occur in the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to apply the changes.</param>
    public virtual void ApplyEvent(IDomainEvent @event)
    {
        var eventType = @event.GetType();
        if (!applyMethodsCache.TryGetValue(eventType, out var applyMethod))
        {
            applyMethod = GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance, null, [eventType], null);

            if (applyMethod != null)
                applyMethodsCache[eventType] = applyMethod;
        }

        applyMethod?.Invoke(this, [@event]);
    }

    /// <summary>
    /// Rehydrate the aggregate root from the events that have occurred.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    /// <param name="id">The identifier of the aggregate root.</param>
    /// <param name="events">The events that have occurred in the aggregate root.</param>
    /// <returns>The aggregate root rehydrated from the events that have occurred.</returns>
    public static TAggregate Rehydrate<TAggregate>(Guid id, IEnumerable<IDomainEvent> events)
       where TAggregate : AggregateRoot
    {
        var aggregate = CreateOrGetDelegate<TAggregate>()(id);

        foreach (var @event in events)
        {
            aggregate.ApplyEvent(@event);
            aggregate.Version++;
        }

        return aggregate;
    }

    /// <summary>
    /// Create an instance of the aggregate root.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root.</typeparam>
    /// <returns>An instance of the aggregate root.</returns>
    private static Func<Guid, T> CreateOrGetDelegate<T>()
    {
        if (!instanceDelegatesCache.TryGetValue(typeof(T), out var instanceDelegate))
        {
            var parameter = Expression.Parameter(typeof(Guid), nameof(IAggregateRoot.Id).ToLower());

            var constructor = typeof(T).GetConstructor([typeof(Guid)]);

            var instante = Expression.New(constructor, parameter);

            var lamda = Expression.Lambda<Func<Guid, T>>(instante, parameter);

            instanceDelegate = lamda.Compile();
            instanceDelegatesCache[typeof(T)] = instanceDelegate;
        }

        return (Func<Guid, T>)instanceDelegate;
    }
}