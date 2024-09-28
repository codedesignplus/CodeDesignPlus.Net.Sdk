namespace CodeDesignPlus.Net.Exceptions.Extensions;

/// <summary>
/// Provides extension methods for string manipulation related to guard clauses.
/// </summary>
public static class GuardExtensions
{
    /// <summary>
    /// Extracts the code from a message string. The code is assumed to be the part before the first colon (":").
    /// </summary>
    /// <param name="message">The message string containing the code and message separated by a colon.</param>
    /// <returns>The extracted code from the message string.</returns>
    public static string GetCode(this string message) => message.Split(":")[0].Trim();

    /// <summary>
    /// Extracts the message from a message string. The message is assumed to be the part after the last colon (":").
    /// </summary>
    /// <param name="message">The message string containing the code and message separated by a colon.</param>
    /// <returns>The extracted message from the message string.</returns>
    public static string GetMessage(this string message) => message.Split(":")[^1].Trim();
}