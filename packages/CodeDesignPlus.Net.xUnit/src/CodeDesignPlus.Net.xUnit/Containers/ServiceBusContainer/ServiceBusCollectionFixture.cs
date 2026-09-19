namespace CodeDesignPlus.Net.xUnit.Containers.ServiceBusContainer;

/// <summary>
/// Provides a fixture for managing an Azure Service Bus emulator container during xUnit tests.
/// </summary>
public sealed class ServiceBusCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the Azure Service Bus tests.
    /// </summary>
    public const string Collection = "ServiceBus Collection";

    /// <summary>
    /// Largest amount of time the fixture waits for the emulator to become ready.
    /// </summary>
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Gets the Azure Service Bus emulator container instance.
    /// </summary>
    public ServiceBusContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusCollectionFixture"/> class.
    /// </summary>
    public ServiceBusCollectionFixture()
    {
        this.Container = new ServiceBusContainer();

        // El emulador arranca su propio SQL Server antes de aceptar conexiones y tarda bastante mas que un
        // broker corriente. Una espera fija seria una carrera: se consulta el endpoint de salud que publica.
        WaitUntilHealthy(this.Container.ManagementPort);
    }

    /// <summary>
    /// Blocks until the emulator reports itself healthy.
    /// </summary>
    /// <param name="managementPort">The host port mapped to the management endpoint.</param>
    /// <exception cref="TimeoutException">The emulator did not become ready in time.</exception>
    private static void WaitUntilHealthy(int managementPort)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        var deadline = DateTime.UtcNow.Add(Timeout);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = client.GetAsync($"http://localhost:{managementPort}/health").GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                    return;
            }
            catch (HttpRequestException)
            {
                // El contenedor todavia no acepta conexiones.
            }
            catch (TaskCanceledException)
            {
                // La peticion expiro mientras el emulador arrancaba.
            }

            Thread.Sleep(2000);
        }

        throw new TimeoutException($"The Azure Service Bus emulator was not ready after {Timeout.TotalSeconds} seconds.");
    }

    /// <summary>
    /// Disposes the Azure Service Bus emulator container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
