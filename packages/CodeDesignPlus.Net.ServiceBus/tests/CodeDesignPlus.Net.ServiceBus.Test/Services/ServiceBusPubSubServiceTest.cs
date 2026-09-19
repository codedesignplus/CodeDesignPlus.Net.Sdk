using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CodeDesignPlus.Net.ServiceBus.Test.Services;

public class ServiceBusPubSubServiceTest
{
    private static ServiceBusPubSubService BuildService(ServiceBusOptions? options = null)
        => new(
            NullLogger<ServiceBusPubSubService>.Instance,
            Mock.Of<IServiceProvider>(),
            Mock.Of<IDomainEventResolver>(),
            Mock.Of<IServiceBusClientProvider>(),
            Mock.Of<ISubscriptionNameResolver>(),
            Mock.Of<IEntityProvisioner>(),
            Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions),
            Microsoft.Extensions.Options.Options.Create(options ?? new ServiceBusOptions
            {
                Enable = true,
                FullyQualifiedNamespace = "contoso.servicebus.windows.net"
            }));

    [Fact]
    public void Constructor_LoggerIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceBusPubSubService(
            null!, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), Mock.Of<IServiceBusClientProvider>(),
            Mock.Of<ISubscriptionNameResolver>(), Mock.Of<IEntityProvisioner>(),
            Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions),
            Microsoft.Extensions.Options.Options.Create(new ServiceBusOptions())));
    }

    [Fact]
    public void Constructor_ClientProviderIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceBusPubSubService(
            NullLogger<ServiceBusPubSubService>.Instance, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), null!,
            Mock.Of<ISubscriptionNameResolver>(), Mock.Of<IEntityProvisioner>(),
            Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions),
            Microsoft.Extensions.Options.Options.Create(new ServiceBusOptions())));
    }

    [Fact]
    public void Constructor_EntityProvisionerIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceBusPubSubService(
            NullLogger<ServiceBusPubSubService>.Instance, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(),
            Mock.Of<IServiceBusClientProvider>(), Mock.Of<ISubscriptionNameResolver>(), null!,
            Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions),
            Microsoft.Extensions.Options.Options.Create(new ServiceBusOptions())));
    }

    [Fact]
    public async Task PublishAsync_EventIsNull_ThrowArgumentNullException()
    {
        var service = BuildService();

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.PublishAsync((IDomainEvent)null!, CancellationToken.None));
    }

    [Theory]
    [InlineData(1, 2000)]
    [InlineData(2, 4000)]
    [InlineData(3, 8000)]
    [InlineData(4, 16000)]
    [InlineData(5, 32000)]
    public void GetRetryDelay_EachDelivery_GrowsExponentially(int deliveryCount, int expectedBaseMs)
    {
        // El pendiente 101: sin esta progresion las once entregas se consumen en milisegundos y un fallo
        // pasajero agota los reintentos antes de que el origen se recupere.
        var service = BuildService();

        var delay = service.GetRetryDelay(deliveryCount);

        Assert.InRange(delay.TotalMilliseconds, expectedBaseMs, expectedBaseMs * 1.2);
    }

    [Fact]
    public void GetRetryDelay_HighDeliveryCount_IsCappedAtMaxRetryInterval()
    {
        var service = BuildService();

        var delay = service.GetRetryDelay(20);

        // El tope evita que la espera supere la renovacion del bloqueo, que es lo que provocaria una reentrega
        // paralela del mismo mensaje.
        Assert.InRange(delay.TotalMilliseconds, 60000, 60000 * 1.2);
    }

    [Fact]
    public void GetRetryDelay_FirstDelivery_UsesTheBaseInterval()
    {
        var service = BuildService();

        var delay = service.GetRetryDelay(0);

        Assert.InRange(delay.TotalMilliseconds, 2000, 2400);
    }

    [Fact]
    public void GetRetryDelay_VeryHighDeliveryCount_DoesNotOverflow()
    {
        var service = BuildService();

        var delay = service.GetRetryDelay(int.MaxValue);

        Assert.InRange(delay.TotalMilliseconds, 60000, 60000 * 1.2);
    }

    [Fact]
    public void MaxMessageSizeBytes_Value_MatchesTheStandardTierLimit()
    {
        Assert.Equal(262144, ServiceBusPubSubService.MaxMessageSizeBytes);
    }
}
