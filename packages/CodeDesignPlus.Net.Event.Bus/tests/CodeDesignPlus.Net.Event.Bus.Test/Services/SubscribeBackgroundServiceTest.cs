using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using Microsoft.Extensions.Hosting;
using CodeDesignPlus.Net.Event.Bus.Extensions;

namespace CodeDesignPlus.Net.Event.Bus.Test.Services;

public class SubscribeBackgroundServiceTest
{
    [Fact]
    public async Task SubscribeEventsHandlers_EventsHandlers_Subscriptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = ConfigurationUtil.GetConfiguration();
        services.AddEventBus(configuration);
        services.AddEventsHandlers<Startup>();

        var serviceProvider = services.BuildServiceProvider();
        var subscriptionManager = serviceProvider.GetRequiredService<ISubscriptionManager>();

        // Act
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        await hostService.StartAsync(CancellationToken.None);

        // Assert
        var subscription = subscriptionManager.FindSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        Assert.NotNull(subscription);
        Assert.Equal(typeof(UserRegisteredEvent), subscription.EventType);
        Assert.Equal(typeof(UserRegisteredEventHandler), subscription.EventHandlerType);
    }
}
