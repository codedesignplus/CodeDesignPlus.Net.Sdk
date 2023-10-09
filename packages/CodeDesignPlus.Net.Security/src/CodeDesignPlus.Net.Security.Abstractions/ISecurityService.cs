namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// This services is the example
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Asynchronously echoes the specified.
    /// </summary>
    /// <param name="value">The value to echo. This value will be echoed to the output stream.</param>
    /// <returns>A that represents the asynchronous echo operation. The value of the TResult parameter contains the echoed value</returns>
    Task<string> EchoAsync(string value);
}
