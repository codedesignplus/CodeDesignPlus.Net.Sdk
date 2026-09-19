namespace CodeDesignPlus.Net.ServiceBus.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddServiceBus_ServicesIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddServiceBus<Startup>(null!, ConfigurationUtil.GetConfiguration()));
    }

    [Fact]
    public void AddServiceBus_ConfigurationIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddServiceBus<Startup>(null!));
    }

    [Fact]
    public void AddServiceBus_SectionNotExist_ThrowServiceBusPubSubException()
    {
        var configuration = ConfigurationUtil.GetConfiguration(new { Core = ConfigurationUtil.CoreOptions });

        var exception = Assert.Throws<ServiceBusPubSubException>(() => new ServiceCollection().AddServiceBus<Startup>(configuration));

        Assert.Equal($"The section {ServiceBusOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddServiceBus_NotEnable_DoesNotRegisterTheTransport()
    {
        // Registrar el transporte con la seccion apagada abriria una conexion contra un namespace que quiza no
        // existe, y el fallo apareceria en el arranque del pod en vez de en la configuracion.
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = ConfigurationUtil.CoreOptions,
            ServiceBus = new ServiceBusOptions { Enable = false }
        });

        var services = new ServiceCollection();

        services.AddServiceBus<Startup>(configuration);

        Assert.DoesNotContain(services, x => x.ServiceType == typeof(IMessage));
        Assert.DoesNotContain(services, x => x.ServiceType == typeof(IServiceBusPubSub));
    }

    [Fact]
    public void AddServiceBus_Enable_RegisterTheTransport()
    {
        var services = new ServiceCollection();

        services.AddServiceBus<Startup>(ConfigurationUtil.GetConfiguration());

        Assert.Contains(services, x => x.ServiceType == typeof(IMessage));
        Assert.Contains(services, x => x.ServiceType == typeof(IServiceBusPubSub));
        Assert.Contains(services, x => x.ServiceType == typeof(IServiceBusClientProvider));
        Assert.Contains(services, x => x.ServiceType == typeof(ISubscriptionNameResolver));
        Assert.Contains(services, x => x.ServiceType == typeof(IEntityProvisioner));
    }

    [Fact]
    public void AddServiceBus_Enable_BindsTheOptions()
    {
        var services = new ServiceCollection();

        services.AddServiceBus<Startup>(ConfigurationUtil.GetConfiguration());

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<ServiceBusOptions>>();

        Assert.True(options.Value.Enable);
        Assert.Equal(ConfigurationUtil.ServiceBusOptions.FullyQualifiedNamespace, options.Value.FullyQualifiedNamespace);
    }

    [Fact]
    public void AddServiceBus_Enable_ResolvesTheSameInstanceForBothContracts()
    {
        // IMessage y IServiceBusPubSub son dos caras del mismo servicio: si se registraran por separado habria
        // dos juegos de procesadores y cada suscripcion se duplicaria.
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddServiceBus<Startup>(ConfigurationUtil.GetConfiguration());

        var provider = services.BuildServiceProvider();

        Assert.Same(provider.GetRequiredService<IMessage>(), provider.GetRequiredService<IServiceBusPubSub>());
    }

    private sealed class Startup { }
}
