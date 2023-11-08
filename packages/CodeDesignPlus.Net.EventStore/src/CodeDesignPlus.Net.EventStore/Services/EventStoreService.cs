
using System.Text;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using EventStore.ClientAPI;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

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

        this.eventStoreFactory = eventStoreFactory ?? throw new ArgumentNullException(nameof(eventStoreFactory));
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

        await connection.AppendToStreamAsync(this.GetAggregateName(metadata), metadata.Version - 1, eventData);
    }

    public async Task<long> GetVersionAsync(string category, Guid aggregateId)
    {
        if(string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var slice = await connection.ReadStreamEventsBackwardAsync(this.GetAggregateName(category, aggregateId), StreamPosition.End, 1, false);

        if (slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0)
            return -1;

        return slice.Events[0].OriginalEventNumber;
    }

    public async Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsAsync(string category, Guid aggregateId)
    {
        if(string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsForwardAsync(this.GetAggregateName(category, aggregateId), 0, 4096, false);

        return streamEvents.Events
            .Select(e =>
            {
                var type = this.types.FirstOrDefault(t => t.Name == e.Event.EventType);

                var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type);
                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return ((IDomainEvent)@event, metadata);
            });
    }

    public async Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId)
        where TAggregate : IAggregateRoot<TUserKey>
    {
        if(string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (aggregateId == Guid.Empty)
            throw new ArgumentException("The provided aggregate ID cannot be an empty GUID.", nameof(aggregateId));

        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var streamEvents = await connection.ReadStreamEventsBackwardAsync(this.GetSnapshotName(category, aggregateId), StreamPosition.End, 1, false);

        if (streamEvents.Status == SliceReadStatus.StreamNotFound || streamEvents.Events.Length == 0)
            return default;

        return JsonConvert.DeserializeObject<TAggregate>(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data), new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver()
        });
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
        await connection.AppendToStreamAsync(this.GetSnapshotName(aggregate.Category, aggregate.Id), ExpectedVersion.Any, eventData);
    }

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

                var @event = (IDomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), type);

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(Encoding.UTF8.GetString(e.Event.Metadata));

                return (@event, metadata);
            });

            events.AddRange(items);

        } while (!currentSlice.IsEndOfStream);

        return events;
    }


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

                var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson);

                var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(metadataJson);

                return (@event, metadata);
            }));

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    public async Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>(string category)
       where TDomainEvent : IDomainEvent
    {
        
        if(string.IsNullOrEmpty(category))
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

                    var @event = JsonConvert.DeserializeObject<TDomainEvent>(@eventJson);
                    var metadata = JsonConvert.DeserializeObject<Metadata<TUserKey>>(metadataJson);

                    events.Add((@event, metadata));
                }
            }

        } while (!currentSlice.IsEndOfStream);

        return events;
    }

    private string GetAggregateName(string category, Guid aggregateId) => $"{category}-{aggregateId}";

    private string GetAggregateName(Metadata<TUserKey> metadata) => $"{metadata.Category}-{metadata.AggregateId}";

    private string GetSnapshotName(string category, Guid aggregateId) => $"{this.GetAggregateName(category, aggregateId)}-{this.options.SnapshotSuffix}";

}

public class PrivateResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        if (!prop.Writable)
        {
            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
        }
        return prop;
    }
}