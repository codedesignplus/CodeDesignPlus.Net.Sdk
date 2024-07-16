using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CodeDesignPlus.Net.Security.Extensions;

public static class AuthenticationFailedContextExtensions
{
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

    private static async Task WriteTokenException(this AuthenticationFailedContext context, string header, string message)
    {
        context.Response.Headers.Append(header, "true");
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { Message = message }));
    }
}