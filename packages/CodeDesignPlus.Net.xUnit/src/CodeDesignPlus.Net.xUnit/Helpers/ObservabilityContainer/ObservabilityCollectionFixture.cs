namespace CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

/// <summary>
/// Provides a fixture for managing an Observability container during xUnit tests.
/// </summary>
public sealed class ObservabilityCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the Observability tests.
    /// </summary>
    public const string Collection = "Observability Collection";

    /// <summary>
    /// Gets the Observability container instance.
    /// </summary>
    public ObservabilityContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityCollectionFixture"/> class.
    /// </summary>
    public ObservabilityCollectionFixture()
    {
        this.Container = new ObservabilityContainer();
    }

    /// <summary>
    /// Disposes the Observability container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}