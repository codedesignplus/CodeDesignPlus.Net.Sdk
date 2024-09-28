using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Claims = CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Authentication;

/// <summary>
/// A test authentication handler for creating a mock authenticated user.
/// </summary>
/// <param name="options">The options monitor for authentication scheme options.</param>
/// <param name="logger">The logger factory.</param>
/// <param name="encoder">The URL encoder.</param>
public class AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// Handles the authentication process and creates a mock authenticated user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(Claims.ClaimTypes.ObjectIdentifier, Guid.NewGuid().ToString()),
            new Claim(Claims.ClaimTypes.Audience, "TestAudience"),
            new Claim(Claims.ClaimTypes.Name, "CodeDesignPlus"),
            new Claim(Claims.ClaimTypes.Emails, "codedesignplus@codedesignplus.com"),
            new Claim(Claims.ClaimTypes.FirstName, "Code"),
            new Claim(Claims.ClaimTypes.LastName, "DesignPlus"),
            new Claim(Claims.ClaimTypes.City, "Mexico City"),
            new Claim(Claims.ClaimTypes.Country, "Mexico"),
            new Claim(Claims.ClaimTypes.PostalCode, "12345"),
            new Claim(Claims.ClaimTypes.StreetAddress , "Street 123"),
            new Claim(Claims.ClaimTypes.State, "Mexico"),
            new Claim(Claims.ClaimTypes.JobTitle, "Developer"),
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestAuthType");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}