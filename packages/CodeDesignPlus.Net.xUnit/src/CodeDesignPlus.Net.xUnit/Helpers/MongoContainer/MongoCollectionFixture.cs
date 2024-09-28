namespace CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;

/// <summary>
/// Provides a fixture for managing a MongoDB container during xUnit tests.
/// </summary>
public sealed class MongoCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the MongoDB tests.
    /// </summary>
    public const string Collection = "Mongo Collection";

    /// <summary>
    /// Gets the MongoDB container instance.
    /// </summary>
    public MongoContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoCollectionFixture"/> class.
    /// </summary>
    public MongoCollectionFixture()
    {
        this.Container = new MongoContainer();
    }

    /// <summary>
    /// Disposes the MongoDB container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}