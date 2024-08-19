namespace CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

/// <summary>
/// Provides a fixture for managing a Kafka container during xUnit tests.
/// </summary>
public sealed class KafkaCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the Kafka tests.
    /// </summary>
    public const string Collection = "Kafka Collection";

    /// <summary>
    /// Gets the Kafka container instance.
    /// </summary>
    public KafkaContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaCollectionFixture"/> class.
    /// </summary>
    public KafkaCollectionFixture()
    {
        this.Container = new KafkaContainer();

        // Wait for the Kafka container to be fully initialized.
        Thread.Sleep(10000);
    }

    /// <summary>
    /// Disposes the Kafka container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}