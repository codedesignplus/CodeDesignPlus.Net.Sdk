namespace CodeDesignPlus.Net.Security.Extensions;

/// <summary>
/// Provides extension methods for handling authentication failures in the context of security token exceptions.
/// </summary>
public static class AuthenticationFailedContextExtensions
{
    /// <summary>
    /// Handles different types of security token exceptions and writes appropriate responses.
    /// </summary>
    /// <param name="context">The authentication failed context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task HandleTokenException(this AuthenticationFailedContext context, Exception exception)
    {
        switch (exception)
        {
            case SecurityTokenExpiredException { Message: var message }:
                await context.WriteTokenException("Token-Expired", message);
                break;
            case SecurityTokenInvalidAudienceException { Message: var message }:
                await context.WriteTokenException("Token-InvalidAudience", message);
                break;
            case SecurityTokenInvalidIssuerException { Message: var message }:
                await context.WriteTokenException("Token-InvalidIssuer", message);
                break;
            case SecurityTokenValidationException { Message: var message }:
                await context.WriteTokenException("Token-Validation", message);
                break;
            case SecurityTokenException { Message: var message }:
                await context.WriteTokenException("Token-Exception", message);
                break;
            default:
                await context.WriteTokenException("Internal Error", "An internal error occurred.");
                break;
        }
    }

    /// <summary>
    /// Writes a token exception response with a specified header and message.
    /// </summary>
    /// <param name="context">The authentication failed context.</param>
    /// <param name="header">The header to append to the response.</param>
    /// <param name="message">The message to write to the response.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task WriteTokenException(this AuthenticationFailedContext context, string header, string message)
    {
        context.Response.Headers.Append(header, "true");
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { Message = message }));
    }
}