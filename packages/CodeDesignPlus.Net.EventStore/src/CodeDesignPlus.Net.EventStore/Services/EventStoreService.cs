
using System.Text;
using CodeDesignPlus.Net.Core.Abstractions;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using CodeDesignPlus.Net.Event.Sourcing.Options;
using System.Reflection.Metadata;

namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides functionality to interact with the event store in an Event Sourcing system.
/// </summary>
public class EventStoreService : IEventStoreService
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ILogger<EventStoreService> logger;
    private readonly EventSourcingOptions options;
    private readonly List<Type> types;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService"/> class.
    /// </summary>
    /// <param name="eventStoreFactory">The factory to create event store connections.</param>
    /// <param name="logger">The logger to use for logging operations and errors.</param>
    /// <param name="options">The configuration options for event sourcing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventStoreFactory"/>, <paramref name="logger"/>, or <paramref name="options"/> is null.</exception>
    public EventStoreService(IEventStoreFactory eventStoreFactory, ILogger<EventStoreService> logger, IOptions<EventSourcingOptions> options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        this.eventStoreFactory = eventStoreFactory;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options.Value;

        this.types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();
    }

    /// <summary>
    /// Appends a domain event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return AppendEventInternalAsync(@event, null);
    }

    /// <summary>
    /// Appends a domain event, with associated metadata, to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="event">The domain event to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
    public Task AppendEventAsync<TDomainEvent, TMetadata>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return AppendEventInternalAsync(@event, @event.Metadata);
    }

    /// <summary>
    /// Appends a domain event to the event store, handling the serialization of the event and its metadata.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to be appended.</typeparam>
    /// <param name="@event">The domain event to append.</param>
    /// <param name="metadata">Optional metadata associated with the event. This can be null if there's no metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="@event"/> is null.</exception>
    private async Task AppendEventInternalAsync<TDomainEvent>(TDomainEvent @event, object metadata)
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var metadataBytes = metadata != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata)) : null;

        var eventData = new EventData(
           Guid.NewGuid(),
           @event.GetType().Name,
           true,
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
           metadataBytes);

        var version = await this.GetAggregateVersionAsync(@event.AggregateId);

        await connection.AppendToStreamAsync(this.GetAggregateName(@event.AggregateId), version, eventData);
    }

    /// <summary>
    /// Gets the version number of an aggregate from the event store.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The aggregate version number.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    public async Task<int> GetAggregateVersionAsync(Guid aggregateId)
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        // Leer los últimos eventos del stream para obtener la versión del agregado.
        var slice = await connection.ReadStreamEventsBackwardAsync(this.GetAggregateName(aggregateId), StreamPosition.End, 1, false);

        // Si no hay eventos, la versión es -1 (no existe).
        if (slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0)
            return -1;

        // Devolver el número de secuencia del último evento.
        return (int)slice.Events[0].OriginalEventNumber;
    }

    /// <summary>
    /// Retrieves the position of the latest event in the global stream.
    /// </summary>
    /// <returns>The position of the last event in the global stream.</returns>
    public async Task<long> GetEventStreamPositionAsync()
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        // Leer el último evento en el stream global para obtener la última posición.
        var slice = await connection.ReadStreamEventsBackwardAsync("$all", StreamPosition.End, 1, false);

        // Si no hay eventos en el stream, devolver -1 (representando que el stream está vacío o no existe).
        if (slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0)
            return -1;

        // Devolver la posición del último evento.
        return slice.Events[0].OriginalPosition.Value.CommitPosition;
    }


    /// <summary>
    /// Loads events for a specific aggregate from the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A collection of domain events for the aggregate.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    public async Task<IEnumerable<IDomainEvent>> LoadEventsForAggregateAsync(Guid aggregateId)
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(this.GetAggregateName(aggregateId), 0, 4096, false);

        return streamEvents.Events
            .Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);

                return (IDomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type);
            });
    }

    /// <summary>
    /// Loads events for a specific aggregate, with associated metadata, from the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A collection of domain events for the aggregate.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    public async Task<IEnumerable<TDomainEvent>> LoadEventsForAggregateAsync<TDomainEvent, TMetadata>(Guid aggregateId)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(this.GetAggregateName(aggregateId), 0, 4096, false);

        return streamEvents.Events.Select(e =>
        {
            var domainEvent = JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));
            domainEvent.SetMetadata(JsonConvert.DeserializeObject<TMetadata>(Encoding.UTF8.GetString(e.Event.Metadata)));

            return domainEvent;
        }).ToList();
    }

    /// <summary>
    /// Loads events starting from a specified position in the global stream.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="position">The position to start reading from.</param>
    /// <returns>A collection of domain events.</returns>
    public async Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent>(long position)
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var slice = await connection.ReadStreamEventsForwardAsync("$all", position, 4096, false);

        return slice.Events
            .Select(e =>
            {
                return JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));
            })
            .ToList();
    }

    /// <summary>
    /// Loads events, with associated metadata, starting from a specified position in the global stream.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="position">The position to start reading from.</param>
    /// <returns>A collection of domain events.</returns>
    public async Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent, TMetadata>(long position)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var slice = await connection.ReadStreamEventsForwardAsync("$all", position, 4096, false);

        return slice.Events.Select(e =>
        {
            var domainEvent = JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));
            domainEvent.SetMetadata(JsonConvert.DeserializeObject<TMetadata>(Encoding.UTF8.GetString(e.Event.Metadata)));

            return domainEvent;
        }).ToList();
    }

    /// <summary>
    /// Loads a snapshot for a specific aggregate from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The latest snapshot of the aggregate, or default value if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    public async Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TKey>
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default; // Or throw an exception if a missing snapshot is considered an error.

        return JsonConvert.DeserializeObject<TAggregate>(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data));
    }

    /// <summary>
    /// Loads a snapshot for a specific aggregate, with associated metadata, from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <typeparam name="TUserKey">The type of the associated user's identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The latest snapshot of the aggregate, or default value if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    public async Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey, TUserKey>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default; // Or throw an exception if a missing snapshot is considered an error.

        return JsonConvert.DeserializeObject<TAggregate>(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data));
    }

    /// <summary>
    /// Saves a snapshot of a specific aggregate to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <param name="aggregate">The current state of the aggregate to be saved as a snapshot.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    public async Task SaveSnapshotAsync<TAggregate, TKey>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TKey>
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        // Serializar el estado del agregado para almacenar como snapshot.
        var serializedAggregate = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aggregate));

        var eventData = new EventData(Guid.NewGuid(), "snapshot", true, serializedAggregate, null);

        // Guardar el snapshot en un stream específico para snapshots.
        await connection.AppendToStreamAsync(this.GetSnapshotName(aggregate.Id), ExpectedVersion.Any, eventData);
    }

    /// <summary>
    /// Saves a snapshot of a specific aggregate, with associated metadata, to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <typeparam name="TUserKey">The type of the associated user's identifier.</typeparam>
    /// <param name="aggregate">The current state of the aggregate to be saved as a snapshot.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    public async Task SaveSnapshotAsync<TAggregate, TKey, TUserKey>(TAggregate aggregate) where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        // Serializar el estado del agregado para almacenar como snapshot.
        var serializedAggregate = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aggregate));

        var eventData = new EventData(Guid.NewGuid(), "snapshot", true, serializedAggregate, null);

        // Suponiendo que 'aggregate.Id' es de tipo TKey, y es el identificador principal del agregado.
        await connection.AppendToStreamAsync(this.GetSnapshotName(aggregate.Id), ExpectedVersion.Any, eventData);
    }

    /// <summary>
    /// Searches for events in the event store by the specified stream name.
    /// </summary>
    /// <param name="streamName">The name of the stream to search in.</param>
    /// <returns>A collection of domain events that match the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="streamName"/> is null or empty.</exception>
    public async Task<IEnumerable<IDomainEvent>> SearchEventsByStreamAsync(string streamName)
    {
        if (string.IsNullOrEmpty(streamName))
            throw new ArgumentNullException(nameof(streamName));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<IDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e => JsonConvert.DeserializeObject<IDomainEvent>(Encoding.UTF8.GetString(e.Event.Data))));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events in the event store by the type of the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to search for.</typeparam>
    /// <returns>A collection of domain events that match the specified type.</returns>
    public async Task<IEnumerable<TDomainEvent>> SearchEventsByEventTypeAsync<TDomainEvent>()
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<TDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$et-{typeof(TDomainEvent).Name}", nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e => JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data))));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events in the event store by the specified category.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to search for.</typeparam>
    /// <param name="category">The category to search in.</param>
    /// <returns>A collection of domain events that match the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="category"/> is null or empty.</exception>
    public async Task<IEnumerable<TDomainEvent>> SearchEventsByCategoryAsync<TDomainEvent>(string category)
       where TDomainEvent : IDomainEvent
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<TDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$ce-{category}", nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e => JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data))));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Generates the aggregate stream name based on the provided aggregate identifier and configured main name.
    /// </summary>
    /// <typeparam name="TKey">The type of the aggregate identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The generated aggregate stream name.</returns>
    private string GetAggregateName<TKey>(TKey aggregateId) => $"{this.options.MainName}-{aggregateId}";

    /// <summary>
    /// Generates the snapshot stream name for an aggregate, based on the provided aggregate identifier and configured snapshot suffix.
    /// </summary>
    /// <typeparam name="TKey">The type of the aggregate identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The generated snapshot stream name.</returns>
    private string GetSnapshotName<TKey>(TKey aggregateId) => $"{this.GetAggregateName(aggregateId)}-{this.options.SnapshotSuffix}";
}
