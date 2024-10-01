namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Logger;

/// <summary>
/// An in-memory logger that stores log messages in a list.
/// </summary>
public class InMemoryLogger() : ILogger
{
    private readonly List<string> logs = [];

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The state to begin scope for.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state) => null;

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>True if the log level is enabled; otherwise, false.</returns>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="state">The state object.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="formatter">The function to create a log message from the state and exception.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        logs.Add(formatter(state, exception));
    }

    /// <summary>
    /// Gets the list of log messages.
    /// </summary>
    public List<string> Logs => logs;
}