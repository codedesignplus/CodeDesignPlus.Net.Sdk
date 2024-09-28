namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Logger;

/// <summary>
/// A logger factory that uses an in-memory logger provider.
/// </summary>
/// <param name="provider">The in-memory logger provider.</param>
public class InMemoryLoggerFactory(InMemoryLoggerProvider provider) : ILoggerFactory
{
    /// <summary>
    /// Adds a logger provider to the factory.
    /// </summary>
    /// <param name="provider">The logger provider to add.</param>
    public void AddProvider(ILoggerProvider provider)
    {
        
    }

    /// <summary>
    /// Creates a logger with the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>An instance of <see cref="ILogger"/>.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return provider.CreateLogger(categoryName);
    }

    /// <summary>
    /// Disposes the logger factory and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        provider.Dispose();
        GC.SuppressFinalize(this);
    }
}