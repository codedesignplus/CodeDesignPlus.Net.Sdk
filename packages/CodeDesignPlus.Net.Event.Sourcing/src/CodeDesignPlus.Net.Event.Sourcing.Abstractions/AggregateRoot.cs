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
    /// <param name="id">The identifier of the aggregate root.</param>
    protected AggregateRoot(Guid id) : base(id) { }

    /// <summary>
    /// Add a domain event to the list of events that have occurred in the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to add to the list of events that have occurred in the aggregate root.</param>
    public override void AddEvent(IDomainEvent @event)
    {
        @event.Metadata.Add("Version", ++this.Version);
        @event.Metadata.Add("Category", this.Category);

        this.DomainEvents.Add(@event);

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

// // Supongamos que T tiene un constructor que acepta ciertos argumentos
// var parametro1 = Expression.Parameter(typeof(TipoDelArgumento1), "nombreDelParametro1");
// var parametro2 = Expression.Parameter(typeof(TipoDelArgumento2), "nombreDelParametro2");

// var constructorInfo = typeof(T).GetConstructor(new[] { typeof(TipoDelArgumento1), typeof(TipoDelArgumento2) });

// var nuevaInstancia = Expression.New(constructorInfo, parametro1, parametro2);

// var lambda = Expression.Lambda<Func<T, TipoDelArgumento1, TipoDelArgumento2>>(nuevaInstancia, parametro1, parametro2);

// var compiledLambda = lambda.Compile();

// // // Uso de la lambda con argumentos
// var instanciaDelTipoT = compiledLambda(instanciaExistenteDeT, valorDelArgumento1, valorDelArgumento2);
