namespace CodeDesignPlus.Net.Exceptions.Models;

/// <summary>
/// Represents an error response containing a trace identifier, layer, and a list of error details.
/// </summary>
/// <param name="TraceId">The trace identifier for the error response.</param>
/// <param name="Layer">The layer where the error occurred.</param>
public record ErrorResponse(string TraceId, Layer Layer)
{
    /// <summary>
    /// Gets the list of error details associated with the error response.
    /// </summary>
    public List<ErrorDetail> Errors { get; } = [];

    /// <summary>
    /// Adds an error detail to the error response.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="field">The field associated with the error.</param>
    public void AddError(string code, string message, string field)
    {
        this.Errors.Add(new ErrorDetail(code, field, message));
    }
}