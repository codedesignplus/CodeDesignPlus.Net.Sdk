using CodeDesignPlus.Net.Security.Abstractions.Options;
using Microsoft.AspNetCore.Http;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Provide the information of the authenticated user during the request
/// </summary>
/// <typeparam name="TKeyUser">The type of the user identifier.</typeparam>
/// <typeparam name="TTenant">The type of the tenant.</typeparam>
/// <param name="httpContextAccessor">The http context accessor.</param>
/// <param name="options">The options.</param>
public class UserContext<TKeyUser, TTenant>(IHttpContextAccessor httpContextAccessor, IOptions<SecurityOptions> options) : IUserContext<TKeyUser, TTenant>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly SecurityOptions options = options.Value;

    /// <summary>
    /// Gets a value boolean that indicates whether is a application
    /// </summary>
    public bool IsApplication => this.options.Applications.Contains(this.GetClaim<string>(ClaimTypes.Audience));
    /// <summary>
    /// Gets the Id User authenticated
    /// </summary>
    public TKeyUser IdUser => this.GetClaim<TKeyUser>(ClaimTypes.ObjectIdentifier);
    /// <summary>
    /// Gets a value that indicates whether the user has been authenticated.
    /// </summary>
    public bool IsAuthenticated => this.User.Identity.IsAuthenticated;
    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    public string Name => this.GetClaim<string>(ClaimTypes.Name);
    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    public string[] Emails => this.GetClaim<string[]>(ClaimTypes.Emails);
    /// <summary>
    /// Get or set the tenant user
    /// </summary>
    public TTenant Tenant => this.GetHeader<TTenant>("X-Tenant");

    /// <summary>
    /// Gets the claims-principal with the user information
    /// </summary>
    public System.Security.Claims.ClaimsPrincipal User { get => this.httpContextAccessor.HttpContext.User; }
    /// <summary>
    /// Gets the first name of the current user.
    /// </summary>
    public string FirstName => this.GetClaim<string>(ClaimTypes.FirstName);
    /// <summary>
    /// Gets the last name of the current user.
    /// </summary>
    public string LastName => this.GetClaim<string>(ClaimTypes.LastName);
    /// <summary>
    /// Gets the phone number of the current user.
    /// </summary>
    public string City => this.GetClaim<string>(ClaimTypes.City);
    /// <summary>
    /// Gets the country of the current user.
    /// </summary>
    public string Country => this.GetClaim<string>(ClaimTypes.Country);
    /// <summary>
    /// Gets the postal code of the current user.
    /// </summary>
    public string PostalCode => this.GetClaim<string>(ClaimTypes.PostalCode);
    /// <summary>
    /// Gets the street address of the current user.
    /// </summary>
    public string StreetAddress => this.GetClaim<string>(ClaimTypes.StreetAddress);
    /// <summary>
    /// Gets the state of the current user.
    /// </summary>
    public string State => this.GetClaim<string>(ClaimTypes.State);
    /// <summary>
    /// Gets the user's job title.
    /// </summary>
    public string JobTitle => this.GetClaim<string>(ClaimTypes.JobTitle);

    /// <summary>
    /// Gets the claim value of the authenticated user
    /// </summary>
    /// <typeparam name="TValue">Type of data that the claim will return</typeparam>
    /// <param name="claimType">Type of claim to get</param>
    /// <returns>Claim value</returns>
    public TValue GetClaim<TValue>(string claimType)
    {
        var claimValue = this.User.FindFirst(claimType)?.Value;

        if (typeof(TValue) == typeof(Guid) && Guid.TryParse(claimValue, out var guidValue))
        {
            return (TValue)(object)guidValue;
        }

        if (typeof(TValue) == typeof(string[]))
        {
            return (TValue)(object)new string[] { claimValue };
        }

        return (TValue)Convert.ChangeType(claimValue, typeof(TValue));
    }

    /// <summary>
    /// Gets the header value of the response
    /// </summary>
    /// <typeparam name="TValue">Type of data that the header will return</typeparam>
    /// <param name="header">Name of the header to get</param>
    /// <returns>Header value</returns>
    public TValue GetHeader<TValue>(string header)
    {
        if (this.httpContextAccessor.HttpContext.Request.Headers.TryGetValue(header, out var values))
        {
            var headerValue = values.FirstOrDefault();

            if (typeof(TValue) == typeof(Guid) && Guid.TryParse(headerValue, out var guidValue))
            {
                return (TValue)(object)guidValue;
            }

            return (TValue)Convert.ChangeType(headerValue, typeof(TValue));
        }

        return default;
    }
}
