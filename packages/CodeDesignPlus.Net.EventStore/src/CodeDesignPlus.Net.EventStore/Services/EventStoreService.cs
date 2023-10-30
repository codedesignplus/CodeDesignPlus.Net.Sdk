
using System.Text;
using CodeDesignPlus.Net.Core.Abstractions;
using EventStore.ClientAPI;
using CodeDesignPlus.Net.Event.Sourcing.Options;
using Newtonsoft.Json;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides functionality to interact with the event store in an Event Sourcing system.
/// </summary>
public class EventStoreService<TUserKey> : IEventStoreService<TUserKey>
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ILogger<EventStoreService<TUserKey>> logger;
    private readonly EventSourcingOptions options;
    private readonly List<Type> types;

    public EventStoreService(IEventStoreFactory eventStoreFactory, ILogger<EventStoreService<TUserKey>> logger, IOptions<EventSourcingOptions> options)
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

    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata)
        where TDomainEvent : IDomainEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return AppendEventInternalAsync(@event, metadata);
    }

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

        await connection.AppendToStreamAsync(this.GetAggregateName(@event.AggregateId), metadata.Version - 1, eventData);
    }

    public async Task<long> GetAggregateVersionAsync(Guid aggregateId)
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
        return slice.Events[0].OriginalEventNumber;
    }

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

    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsForAggregateAsync(Guid aggregateId)
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(this.GetAggregateName(aggregateId), 0, 4096, false);

        return streamEvents.Events
            .Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);

                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type);
                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return ((IDomainEvent)@event, metadata);
            });
    }

    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsFromPositionAsync(long position)
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var slice = await connection.ReadStreamEventsForwardAsync("$all", position, 4096, false);

        return slice.Events
            .Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);
                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type);
                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return ((IDomainEvent)@event, metadata);
            })
            .ToList();
    }

    public async Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TUserKey>
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default; // Or throw an exception if a missing snapshot is considered an error.

        return JsonConvert.DeserializeObject<TAggregate>(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data));
    }

    public async Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TUserKey>
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

    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> SearchEventsByStreamAsync(string streamName)
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
                var @event = JsonConvert.DeserializeObject<IDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return (@event, metadata);
            });

            events.AddRange(items);

        } while (!currentSlice.IsEndOfStream);

        return events;
    }


    public async Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsByEventTypeAsync<TDomainEvent>()
        where TDomainEvent : IDomainEvent
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var events = new List<(TDomainEvent, Metadata<TUserKey>)>();
        StreamEventsSlice currentSlice;
        var nextSliceStart = (long)StreamPosition.Start;
        do
        {
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$et-{typeof(TDomainEvent).Name}", nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e =>
            {

                var @event = JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return (@event, metadata);
            }));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    public async Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsByCategoryAsync<TDomainEvent>(string category)
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
            currentSlice = await connection.ReadStreamEventsForwardAsync($"$ce-{category}", nextSliceStart, 200, false);
            nextSliceStart = currentSlice.NextEventNumber;

            events.AddRange(currentSlice.Events.Select(e =>
            {

                var @event = JsonConvert.DeserializeObject<TDomainEvent>(Encoding.UTF8.GetString(e.Event.Data));

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return (@event, metadata);
            }));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }
    private string GetAggregateName<TKey>(TKey aggregateId) => $"{this.options.MainName}-{aggregateId}";

    private string GetSnapshotName<TKey>(TKey aggregateId) => $"{this.GetAggregateName(aggregateId)}-{this.options.SnapshotSuffix}";
}
