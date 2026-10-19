namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Services;

/// <summary>
/// Un logger que guarda lo que se registra, para las clases internas que Moq no puede simular.
/// </summary>
public sealed class ListLogger<T> : ILogger<T>
{
    private readonly List<(LogLevel Level, string Message)> entries = [];

    /// <summary>
    /// Lo registrado, con su nivel.
    /// </summary>
    public IReadOnlyList<(LogLevel Level, string Message)> Entries
    {
        get
        {
            lock (this.entries)
            {
                return [.. this.entries];
            }
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (this.entries)
        {
            this.entries.Add((logLevel, formatter(state, exception)));
        }
    }
}
