using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Logger;

/// <summary>
/// A logger provider that creates and manages in-memory loggers.
/// </summary>
public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InMemoryLogger> loggers = new();
    
    /// <summary>
    /// Gets the collection of in-memory loggers.
    /// </summary>
    public ConcurrentDictionary<string, InMemoryLogger> Loggers => loggers;

    /// <summary>
    /// Creates a logger with the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>An instance of <see cref="ILogger"/>.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, _ => new InMemoryLogger());
    }

    /// <summary>
    /// Disposes the logger provider and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        loggers.Clear();
        GC.SuppressFinalize(this);
    }
}