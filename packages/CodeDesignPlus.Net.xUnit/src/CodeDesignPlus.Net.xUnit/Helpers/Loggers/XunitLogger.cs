using System.Text;
using Xunit.Abstractions;

namespace CodeDesignPlus.Net.xUnit.Helpers.Loggers;

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

    public XunitLogger(ITestOutputHelper output, IExternalScopeProvider scopes, string categoryName, bool useScopes)
    {
        _output = output;
        _scopes = scopes;
        _categoryName = categoryName;
        _useScopes = useScopes;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _scopes.Push(state);
    }

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
                break;
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