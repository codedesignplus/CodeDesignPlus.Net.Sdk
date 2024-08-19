namespace CodeDesignPlus.Net.xUnit.Helpers.Loggers;

/// <summary>
/// Provides an xUnit logger provider that creates instances of <see cref="XunitLogger"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="XunitLoggerProvider"/> class.
/// </remarks>
/// <param name="output">The xUnit test output helper.</param>
/// <param name="useScopes">A value indicating whether to use scopes.</param>
public sealed class XunitLoggerProvider(ITestOutputHelper output, bool useScopes) : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper _output = output;
    private readonly bool _useScopes = useScopes;
    private IExternalScopeProvider _scopes;

    /// <summary>
    /// Creates a new <see cref="XunitLogger"/> instance.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>A new instance of <see cref="XunitLogger"/>.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(_output, _scopes, categoryName, _useScopes);
    }

    /// <summary>
    /// Disposes the logger provider.
    /// </summary>
    public void Dispose()
    {
        // No resources to dispose.
    }

    /// <summary>
    /// Sets the external scope provider.
    /// </summary>
    /// <param name="scopes">The external scope provider to set.</param>
    public void SetScopeProvider(IExternalScopeProvider scopes)
    {
        _scopes = scopes;
    }
}