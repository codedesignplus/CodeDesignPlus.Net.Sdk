namespace CodeDesignPlus.Net.xUnit.Helpers.Loggers;

/// <summary>
/// A logger implementation for xUnit that writes log messages to the test output.
/// </summary>
public sealed class XunitLogger : ILogger
{
    private const string ScopeDelimiter = "=> ";
    private const string Spacer = "      ";

    private const string Trace = "trce";
    private const string Debug = "dbug";
    private const string Info = "info";
    private const string Warn = "warn";
    private const string Error = "fail";
    private const string Critical = "crit";

    private readonly string _categoryName;
    private readonly bool _useScopes;
    private readonly ITestOutputHelper _output;
    private readonly IExternalScopeProvider _scopes;

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitLogger"/> class.
    /// </summary>
    /// <param name="output">The xUnit test output helper.</param>
    /// <param name="scopes">The external scope provider.</param>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <param name="useScopes">A value indicating whether to use scopes.</param>
    public XunitLogger(ITestOutputHelper output, IExternalScopeProvider scopes, string categoryName, bool useScopes)
    {
        _output = output;
        _scopes = scopes;
        _categoryName = categoryName;
        _useScopes = useScopes;
    }

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns><c>true</c> if the log level is enabled; otherwise, <c>false</c>.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state)
    {
        return _scopes.Push(state);
    }

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logLevel">The level of the log entry.</param>
    /// <param name="eventId">The event ID associated with the log entry.</param>
    /// <param name="state">The object to be written.</param>
    /// <param name="exception">The exception related to this log entry.</param>
    /// <param name="formatter">The function to create a <see cref="string"/> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var sb = new StringBuilder();

        switch (logLevel)
        {
            case LogLevel.Trace:
                sb.Append(Trace);
                break;
            case LogLevel.Debug:
                sb.Append(Debug);
                break;
            case LogLevel.Information:
                sb.Append(Info);
                break;
            case LogLevel.Warning:
                sb.Append(Warn);
                break;
            case LogLevel.Error:
                sb.Append(Error);
                break;
            case LogLevel.Critical:
                sb.Append(Critical);
                break;
            case LogLevel.None:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }

        sb.Append(": ").Append(_categoryName).Append('[').Append(eventId).Append(']').AppendLine();

        if (_useScopes && TryAppendScopes(sb))
            sb.AppendLine();

        sb.Append(Spacer);
        sb.Append(formatter(state, exception));

        if (exception != null)
        {
            sb.AppendLine();
            sb.Append(Spacer);
            sb.Append(exception);
        }

        var message = sb.ToString();

        try
        {
            _output.WriteLine(message);
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Attempts to append the current scopes to the log message.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the scopes to.</param>
    /// <returns><c>true</c> if scopes were appended; otherwise, <c>false</c>.</returns>
    private bool TryAppendScopes(StringBuilder sb)
    {
        var scopes = false;
        _scopes.ForEachScope((callback, state) =>
        {
            if (!scopes)
            {
                state.Append(Spacer);
                scopes = true;
            }
            state.Append(ScopeDelimiter);
            state.Append(callback);
        }, sb);
        return scopes;
    }
}