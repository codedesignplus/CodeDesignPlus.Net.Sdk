
namespace CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

/// <summary>
/// Provides a Redis container for the test collection. 
/// </summary>
public sealed class RedisCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection.
    /// </summary>
    public const string Collection = "Redis Collection";

    /// <summary>
    /// Gets the MongoDB container instance.
    /// </summary>
    public RedisContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCollectionFixture"/> class.
    /// </summary>
    public RedisCollectionFixture()
    {
        this.Container = new RedisContainer();
    }

    /// <summary>
    /// Disposes the MongoDB container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}