
using System.Text;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using EventStore.ClientAPI;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;
using Newtonsoft.Json;
using CodeDesignPlus.Net.EventStore.Core;

namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides services for interacting with EventStore.
/// </summary>
public class EventStoreService<TUserKey> : IEventStoreService<TUserKey>
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ILogger<EventStoreService<TUserKey>> logger;
    private readonly EventSourcingOptions options;
    private readonly List<Type> types;
    private readonly JsonSerializerSettings jsonSerializerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService{TUserKey}"/> class.
    /// </summary>
    /// <param name="eventStoreFactory">The factory for creating EventStore connections.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options for event sourcing.</param>
    public EventStoreService(IEventStoreFactory eventStoreFactory, ILogger<EventStoreService<TUserKey>> logger, IOptions<EventSourcingOptions> options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        this.eventStoreFactory = eventStoreFactory ?? throw new ArgumentNullException(nameof(eventStoreFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options.Value;

        this.jsonSerializerSettings = new()
        {
            ContractResolver = new PrivateResolver()
        };

        this.types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();
    }

    /// <summary>
    /// Counts the number of events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of events.</returns>
    public Task<long> CountEventsAsync(string category, Guid aggregateId)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        return CountEventsInternalAsync(category, aggregateId);
    }

    /// <summary>
    /// Counts the number of events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of events.</returns>
    private async Task<long> CountEventsInternalAsync(string category, Guid aggregateId)
    {
        var connection = await eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

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
    /// <param name="event">The event to append.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata)
        where TDomainEvent : IDomainEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return AppendEventInternalAsync(@event, metadata);
    }

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <param name="event">The event to append.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task AppendEventInternalAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata)
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var eventData = new EventData(
           Guid.NewGuid(),
           @event.GetType().Name,
           true,
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata)));

        await connection.AppendToStreamAsync(GetAggregateName(metadata), metadata.Version - 1, eventData);
    }

    /// <summary>
    /// Gets the version of the event store.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the version of the event store.</returns>
    public async Task<long> GetVersionAsync(string category, Guid aggregateId)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

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
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded events.</returns>
    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsAsync(string category, Guid aggregateId)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(GetAggregateName(category, aggregateId), 0, 4096, false);

        return streamEvents.Events
            .Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);

                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type, this.jsonSerializerSettings);
                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata), this.jsonSerializerSettings);

                return ((IDomainEvent)@event, metadata);
            });
    }

    /// <summary>
    /// Loads the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded snapshot.</returns>
    public async Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId)
        where TAggregate : IAggregateRoot<TUserKey>
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(category, aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default;

        return JsonConvert.DeserializeObject<TAggregate>(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data), this.jsonSerializerSettings);
    }

    /// <summary>
    /// Saves the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TUserKey>
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var serializedAggregate = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aggregate));

        var eventData = new EventData(Guid.NewGuid(), "snapshot", true, serializedAggregate, null);

        await connection.AppendToStreamAsync(this.GetSnapshotName(aggregate.Category, aggregate.Id), ExpectedVersion.Any, eventData);
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria to match.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync(string streamName)
    {
        if (string.IsNullOrEmpty(streamName))
            throw new ArgumentNullException(nameof(streamName));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<(IDomainEvent, Metadata<TUserKey>)>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            var items = currentSlice.Events.Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);

                var @event = (IDomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type, this.jsonSerializerSettings);

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata), this.jsonSerializerSettings);

                return (@event, metadata);
            });

            events.AddRange(items);

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria to match.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>()
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<(TDomainEvent, Metadata<TUserKey>)>();
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

                var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson, this.jsonSerializerSettings);

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(metadataJson, this.jsonSerializerSettings);

                return (@event, metadata);
            }));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria to match.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    public async Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>(string category)
       where TDomainEvent : IDomainEvent
    {

        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<(TDomainEvent, Metadata<TUserKey>)>();
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
                    var metadataJson = Encoding.UTF8.GetString(e.Event.Metadata);

                    var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson, this.jsonSerializerSettings);
                    var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(metadataJson, this.jsonSerializerSettings);

                    events.Add((@event, metadata));
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
    /// Gets the aggregate name using the provided metadata.
    /// </summary>
    /// <param name="metadata">The metadata used to construct the aggregate name.</param>
    /// <returns>The name of the aggregate.</returns>
    private static string GetAggregateName(Metadata<TUserKey> metadata)
    {
        return $"{metadata.Category}-{metadata.AggregateId}";
    }

    /// <summary>
    /// Gets the snapshot name for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <returns>The name of the snapshot.</returns>
    private string GetSnapshotName(string category, Guid aggregateId)
    {
        return $"{GetAggregateName(category, aggregateId)}-{this.options.SnapshotSuffix}";
    }
}