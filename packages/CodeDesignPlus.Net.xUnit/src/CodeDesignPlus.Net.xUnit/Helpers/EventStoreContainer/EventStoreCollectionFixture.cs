namespace CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

/// <summary>
/// Provides a fixture for managing an EventStore container during xUnit tests.
/// </summary>
public sealed class EventStoreCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the EventStore tests.
    /// </summary>
    public const string Collection = "EventStore Collection";

    /// <summary>
    /// Gets the EventStore container instance.
    /// </summary>
    public EventStoreContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreCollectionFixture"/> class.
    /// </summary>
    public EventStoreCollectionFixture()
    {
        this.Container = new EventStoreContainer();
    }

    /// <summary>
    /// Disposes the EventStore container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}