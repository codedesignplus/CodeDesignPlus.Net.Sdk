using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Extensions;
using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.Kafka.Options;
using CodeDesignPlus.Net.Kafka.Services;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using CodeDesignPlus.Net.xUnit;
using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Kafka.Test;

public class KafkaServiceTest : IClassFixture<KafkaContainer>
{
    private readonly KafkaContainer kafkaContainer;
    public KafkaServiceTest(KafkaContainer kafkaContainer)
    {
        this.kafkaContainer = kafkaContainer;
    }

    [Fact]
    public async Task PublishAsync()
    {
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventBus = new
            {
                Enable = true,
                Name = nameof(Event.Bus.Options.EventBusOptions.Name),
                Email = $"{nameof(Event.Bus.Options.EventBusOptions.Name)}@codedesignplus.com"
            },
            Kafka = OptionUtils.KafkaOptions
        });
        var serviceCollection = new ServiceCollection();

        var @event = new UserCreatedEvent()
        {
            Id = 1,
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        serviceCollection
            .AddLogging()
            .AddEventBus(configuration)
            .AddKafka<StartupLogic>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        await eventBus.PublishAsync(@event, CancellationToken.None);

        await Task.Delay(TimeSpan.FromSeconds(5));
        
       // var memoryService = serviceProvider.GetRequiredService<IMemoryService>();
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        _ = hostService.StartAsync(CancellationToken.None);


        await Task.Delay(TimeSpan.FromSeconds(100));
        

    }
}
