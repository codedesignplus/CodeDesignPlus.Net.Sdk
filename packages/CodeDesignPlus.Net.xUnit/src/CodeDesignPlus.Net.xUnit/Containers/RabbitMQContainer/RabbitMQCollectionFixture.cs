namespace CodeDesignPlus.Net.xUnit.Containers.RabbitMQContainer;

/// <summary>
/// Provides a fixture for managing a RabbitMQ container during xUnit tests.
/// </summary>
public sealed class RabbitMQCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the RabbitMQ tests.
    /// </summary>
    public const string Collection = "RabbitMQ Collection";

    /// <summary>
    /// Gets the RabbitMQ container instance.
    /// </summary>
    public RabbitMQContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQCollectionFixture"/> class.
    /// </summary>
    public RabbitMQCollectionFixture()
    {
        this.Container = new RabbitMQContainer();

        // Wait for the RabbitMQ container to be fully initialized.
        Thread.Sleep(10000);
    }

    /// <summary>
    /// Disposes the RabbitMQ container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}