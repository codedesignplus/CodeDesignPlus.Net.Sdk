namespace CodeDesignPlus.Net.xUnit.Helpers.Loggers;

public sealed class XunitLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper _output;
    private readonly bool _useScopes;

    private IExternalScopeProvider _scopes;

    public XunitLoggerProvider(ITestOutputHelper output, bool useScopes)
    {
        _output = output;
        _useScopes = useScopes;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(_output, _scopes, categoryName, _useScopes);
    }

    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopes)
    {
        _scopes = scopes;
    }
}

