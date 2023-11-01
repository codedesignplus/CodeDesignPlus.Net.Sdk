using System.Linq.Expressions;
using System.Reflection;

namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

public abstract class AggregateRootBase<TUserKey> : IAggregateRoot<TUserKey>
{
    private static readonly Dictionary<Type, Delegate> instanceDelegatesCache = new();
    private static readonly Dictionary<Type, MethodInfo> applyMethodsCache = new();
    private readonly List<(IDomainEvent Event, Metadata<TUserKey> Metadata)> uncommittedEvents = new();
    public abstract string Category {get; protected set;}
    public Guid Id { get; set; }
    public long Version { get; set; } = -1;
    private long sequence = -1;

    public virtual void ApplyChange(IDomainEvent @event, TUserKey idUser)
    {
        this.sequence++;
        var metadata = new Metadata<TUserKey>(@event.AggregateId, this.sequence, idUser, Category);
        this.uncommittedEvents.Add((@event, metadata));
        ApplyEvent(@event, metadata);
    }

    public virtual void ApplyEvent(IDomainEvent @event, Metadata<TUserKey> metadata)
    {
        var eventType = @event.GetType();
        if (!applyMethodsCache.TryGetValue(eventType, out var applyMethod))
        {
            applyMethod = GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { eventType, typeof(Metadata<TUserKey>) }, null);
            if (applyMethod != null)
            {
                applyMethodsCache[eventType] = applyMethod;
            }
        }

        applyMethod?.Invoke(this, new object[] { @event, metadata });
    }

    public static TAggregate Rehydrate<TAggregate>(IEnumerable<(IDomainEvent, Metadata<TUserKey>)> events)
       where TAggregate : IAggregateRoot<TUserKey>
    {
        var aggregate = CreateOrGetDelegate<TAggregate>()();

        foreach (var (@event, metadata) in events)
        {
            aggregate.ApplyEvent(@event, metadata);
            aggregate.Version = metadata.Version;
        }

        return aggregate;
    }

    public static Func<T> CreateOrGetDelegate<T>()
    {
        if (!instanceDelegatesCache.TryGetValue(typeof(T), out var instanceDelegate))
        {
            instanceDelegate = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
            instanceDelegatesCache[typeof(T)] = instanceDelegate;
        }

        return (Func<T>)instanceDelegate;
    }

    public IReadOnlyList<(IDomainEvent Event, Metadata<TUserKey> Metadata)> UncommittedEvents => this.uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents() => this.uncommittedEvents.Clear();
}