
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.Serializer;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Text;

namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides services for interacting with EventStore.
/// </summary>
public class EventStoreService : IEventStoreService
{
    private readonly JsonSerializerSettings settings = new()
    {
        ContractResolver = new EventStoreContratResolver(),
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };

    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ILogger<EventStoreService> logger;
    private readonly EventSourcingOptions options;
    private readonly IDomainEventResolverService domainEventResolverService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService"/> class.
    /// </summary>
    /// <param name="eventStoreFactory">The factory for creating EventStore connections.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options for event sourcing.</param>
    /// <param name="domainEventResolverService">The service for resolving domain events.</param>
    /// <exception cref="ArgumentNullException">The eventStoreFactory, logger or options is null.</exception>
    public EventStoreService(IEventStoreFactory eventStoreFactory, IDomainEventResolverService domainEventResolverService, ILogger<EventStoreService> logger, IOptions<EventSourcingOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.eventStoreFactory = eventStoreFactory ?? throw new ArgumentNullException(nameof(eventStoreFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options.Value;
        this.domainEventResolverService = domainEventResolverService ?? throw new ArgumentNullException(nameof(domainEventResolverService));
    }

    /// <summary>
    /// Counts the number of events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of events.</returns>
    public Task<long> CountEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        return CountEventsInternalAsync(category, aggregateId, cancellationToken);
    }

    /// <summary>
    /// Counts the number of events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of events.</returns>
    private async Task<long> CountEventsInternalAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var stream = GetAggregateName(category, aggregateId);

        long count = 0;
        StreamEventsSlice currentSlice;
        long nextSliceStart = StreamPosition.Start;
        const int pageSize = 4096;

        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync(stream, nextSliceStart, pageSize, false);
            nextSliceStart = currentSlice.NextEventNumber;
            count += currentSlice.Events.Length;
        } while (!currentSlice.IsEndOfStream);

        return count;
    }

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <param name="category">The category of the events.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <param name="version">The version of the event store.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AppendEventAsync<TDomainEvent>(string category, TDomainEvent @event, long? version = null, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return AppendEventInternalAsync(category, @event, version, cancellationToken);
    }

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <param name="event">The event to append.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task AppendEventInternalAsync<TDomainEvent>(string category, TDomainEvent @event, long? version = null, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        version ??= await GetVersionAsync(category, @event.AggregateId, cancellationToken);

        var eventData = new EventData(
           @event.EventId,
           @event.EventType,
           true,
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, this.settings)),
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event.Metadata, this.settings)));

        await connection.AppendToStreamAsync(GetAggregateName(category, @event.AggregateId), (long)version, eventData);
    }

    /// <summary>
    /// Gets the version of the event store.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the version of the event store.</returns>
    public async Task<long> GetVersionAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var slice = await connection.ReadStreamEventsBackwardAsync(GetAggregateName(category, aggregateId), StreamPosition.End, 1, false);

        if (slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0)
            return -1;

        return slice.Events[0].OriginalEventNumber;
    }

    /// <summary>
    /// Loads the events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded events.</returns>
    public async Task<IEnumerable<IDomainEvent>> LoadEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(GetAggregateName(category, aggregateId), 0, 4096, false);

        return streamEvents.Events
            .Select(e =>
            {
                var type = domainEventResolverService.GetDomainEventType(e.Event.EventType);

                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type, this.settings);

                return @event as IDomainEvent;
            });
    }

    /// <summary>
    /// Loads the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded snapshot.</returns>
    public async Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : Event.Sourcing.Abstractions.IAggregateRoot
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(category, aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default;

        var json = Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data);

        return JsonConvert.DeserializeObject<TAggregate>(json, this.settings);
    }

    /// <summary>
    /// Saves the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : Event.Sourcing.Abstractions.IAggregateRoot
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var serializedAggregate = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aggregate, this.settings));

        var eventData = new EventData(Guid.NewGuid(), "snapshot", true, serializedAggregate, null);

        await connection.AppendToStreamAsync(this.GetSnapshotName(aggregate.Category, aggregate.Id), ExpectedVersion.Any, eventData);
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="streamName">The name of the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<IDomainEvent>> SearchEventsAsync(string streamName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(streamName))
            throw new ArgumentNullException(nameof(streamName));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var events = new List<IDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            var items = currentSlice.Events.Select(e =>
            {
                var type = domainEventResolverService.GetDomainEventType(e.Event.EventType);

                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type, this.settings);

                return @event as IDomainEvent;
            });

            events.AddRange(items);

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var events = new List<TDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$et-{typeof(TDomainEvent).Name}", nextSliceStart, 200, true);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e =>
            {

                var @eventJson = Encoding.UTF8.GetString(e.Event.Data);
                var metadataJson = Encoding.UTF8.GetString(e.Event.Metadata);

                var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson, this.settings);

                return @event;
            }));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="criteria">The criteria to match.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(string category, CancellationToken cancellationToken = default)
       where TDomainEvent : IDomainEvent
    {

        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken);

        var events = new List<TDomainEvent>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;

        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$ce-{category}", nextSliceStart, 200, true);
            nextSliceStart = currentSlice.NextEventNumber;

            foreach (var e in currentSlice.Events)
            {
                if (e.Event.EventType == typeof(TDomainEvent).Name)
                {
                    var @eventJson = Encoding.UTF8.GetString(e.Event.Data);

                    var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson, this.settings);

                    events.Add(@event);
                }
            }

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Gets the aggregate name for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the aggregate.</param>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <returns>The name of the aggregate.</returns>
    private static string GetAggregateName(string category, Guid aggregateId)
    {
        return $"{category}-{aggregateId}";
    }

    /// <summary>
    /// Gets the snapshot name for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the aggregate.</param>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <returns>The name of the snapshot.</returns>
    private string GetSnapshotName(string category, Guid aggregateId)
    {
        return $"{GetAggregateName(category, aggregateId)}-{this.options.SnapshotSuffix}";
    }
}