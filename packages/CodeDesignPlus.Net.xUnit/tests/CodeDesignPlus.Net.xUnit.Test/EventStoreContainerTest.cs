using CodeDesignPlus.Net.xUnit.Containers.EventStoreContainer;
using EventStore.ClientAPI;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection("EventStore Collection")]
public class EventStoreContainerTest(EventStoreCollectionFixture eventStoreCollectionFixture)
{
    private readonly EventStoreContainer eventStoreContainer = eventStoreCollectionFixture.Container;

    [Fact]
    public async Task CheckConnectionServer()
    {
        var connectionSettings = ConnectionSettings.Create()
           .DisableTls()
           .UseConsoleLogger();

        var connection = EventStoreConnection.Create(connectionSettings, new Uri($"tcp://localhost:{this.eventStoreContainer.Port}"));
        await connection.ConnectAsync();

        // Escribir un evento
        var eventDataExptected = new EventData(
            eventId: Guid.NewGuid(),
            type: "MyEventType",
            isJson: true,
            data: Encoding.UTF8.GetBytes("{\"myProperty\":\"myValue\"}"),
            metadata: null
        );

        await connection.AppendToStreamAsync("MyStream", ExpectedVersion.Any, eventDataExptected);

        // Leer eventos
        var streamEvents = await connection.ReadStreamEventsForwardAsync("MyStream", start: 0, count: 10, resolveLinkTos: false);

        foreach (var resolvedEvent in streamEvents.Events)
        {
            var eventData = resolvedEvent.Event;
            var eventPayload = Encoding.UTF8.GetString(eventData.Data);

            Console.WriteLine($"Event Type: {eventData.EventType}");
            Console.WriteLine($"Data: {eventPayload}");

            Assert.Equal(eventDataExptected.Data, eventData.Data);
        }

        connection.Close();
    }
}
