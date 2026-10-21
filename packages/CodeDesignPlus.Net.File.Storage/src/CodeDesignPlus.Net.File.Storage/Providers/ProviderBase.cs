namespace CodeDesignPlus.Net.File.Storage.Providers;

/// <summary>
/// Abstract base class for file storage providers.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseProvider"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The host environment.</param>
public abstract class BaseProvider(ILogger logger, IHostEnvironment environment)
{
    /// <summary>
    /// The host environment.
    /// </summary>
    protected readonly IHostEnvironment Environment = environment;
    /// <summary>
    /// The logger instance.
    /// </summary>
    protected readonly ILogger Logger = logger;

    /// <summary>
    /// Processes a file operation asynchronously.
    /// </summary>
    /// <param name="isEnable">Indicates whether the operation is enabled.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="typeProviders">The type of provider.</param>
    /// <param name="process">The function to process the file.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task<M.Response> ProcessAsync(bool isEnable, string filename, TypeProviders typeProviders, Func<M.File, M.Response, Task<M.Response>> process)
    {
        if (!isEnable)
            return null;

        var file = new M.File(filename);
        var response = new M.Response(file, typeProviders);

        try
        {
            response = await process(file, response).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while processing the file {Filename}", filename);

            response.Success = false;
            response.Message = Describe(ex);

            if (Environment.IsDevelopment())
                response.Exception = ex;
        }

        return response;
    }

    /// <summary>
    /// Describes a failure in one line: what failed and why, so the caller can log it or carry it in its own error.
    /// </summary>
    /// <remarks>
    /// The type goes first because the message alone hid the cause: an import answered «Object reference not set to an
    /// instance of an object.» with nothing saying that the file had no MIME type. Only the first line of the message is
    /// kept: an Azure storage error appends the response headers, which the caller does not need.
    /// </remarks>
    /// <param name="exception">The failure.</param>
    /// <returns>The description.</returns>
    protected static string Describe(Exception exception)
    {
        var message = FirstLine(exception.Message);

        return exception is Azure.RequestFailedException failed
            ? $"{exception.GetType().Name} ({failed.Status} {failed.ErrorCode}): {message}"
            : $"{exception.GetType().Name}: {message}";
    }

    /// <summary>
    /// The first non-empty line of <paramref name="text"/>.
    /// </summary>
    private static string FirstLine(string text)
    {
        return text?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
    }
}
