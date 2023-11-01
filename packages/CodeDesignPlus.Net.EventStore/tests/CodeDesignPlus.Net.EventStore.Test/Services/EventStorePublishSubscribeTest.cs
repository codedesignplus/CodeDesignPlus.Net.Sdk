using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Test.Services;

public class EventStorePublishSubscribeTest : IClassFixture<EventStoreContainer>
{
    private readonly EventStoreContainer container;
    private readonly ITestOutputHelper testOutput;

    public EventStorePublishSubscribeTest(ITestOutputHelper output, EventStoreContainer container)
    {
        this.container = container;
        this.testOutput = output;

    }

    [Fact]
    public async Task PublishAndSubscribe()
    {
        var testServer = BuildTestServer(true, this.testOutput);

        var eventBus = testServer.Host.Services.GetRequiredService<IEventBus>();

        var @event = new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), Helpers.Domain.OrderStatus.Pending, new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        }, DateTime.UtcNow);

        await eventBus.PublishAsync(@event, CancellationToken.None);


        OrderCreatedEvent? userEvent;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(15));

            userEvent = Startup.MemoryService.OrderCreatedEvent.FirstOrDefault(x => x.IdEvent == @event.IdEvent);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.OrderCreatedEvent);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", @event.IdEvent, userEvent.IdEvent);
        Assert.Equal(@event.AggregateId, userEvent.AggregateId);
        Assert.Equal(@event.Client.Name, userEvent.Client.Name);
        Assert.Equal(@event.Client.Id, userEvent.Client.Id);
        Assert.Equal(@event.EventType, userEvent.EventType);
        Assert.Equal(@event.DateCreated, userEvent.DateCreated);
        Assert.Equal(@event.EventDate, userEvent.EventDate);
        Assert.Equal(@event.OrderStatus, userEvent.OrderStatus);
        Assert.Equal(@event.IdEvent, userEvent.IdEvent);
    }


    private static TestServer BuildTestServer(bool enableQueue, ITestOutputHelper output)
    {
        var webHostBuilder = new WebHostBuilder()
                    .ConfigureLogging(logging =>
                    {
                        // check if scopes are used in normal operation
                        var useScopes = logging.UsesScopes();
                        // remove other logging providers, such as remote loggers or unnecessary event logs
                        logging.ClearProviders();
                        logging.Services.AddSingleton<ILoggerProvider>(r => new XunitLoggerProvider(output, useScopes));
                    })
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        AddJsonStream(config, enableQueue);
                    }).UseStartup<Startup>();

        var testServer = new TestServer(webHostBuilder);
        return testServer;
    }


    private static void AddJsonStream(IConfigurationBuilder config, bool enableQueue)
    {
        var json = JsonSerializer.Serialize(new
        {
            EventBus = new
            {
                EnableQueue = enableQueue,
            },
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.EventStoreOptions
        });

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        config.AddJsonStream(memoryStream);
    }
}
