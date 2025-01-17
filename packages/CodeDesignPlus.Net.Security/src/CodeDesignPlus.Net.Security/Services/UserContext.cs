namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Provides user context information based on the current HTTP context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserContext"/> class.
/// </remarks>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <param name="options">The security options.</param>
public class UserContext(IHttpContextAccessor httpContextAccessor, IOptions<SecurityOptions> options) : IUserContext
{

    /// <summary>
    /// Gets a value indicating whether the current user is an application.
    /// </summary>
    public bool IsApplication => options.Value.Applications.Contains(this.GetClaim<string>(ClaimTypes.Audience));

    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public Guid IdUser => this.GetClaim<Guid>(ClaimTypes.ObjectIdentifier);

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => this.User.Identity.IsAuthenticated;

    /// <summary>
    /// Gets the user's name.
    /// </summary>
    public string Name => this.GetClaim<string>(ClaimTypes.Name);

    /// <summary>
    /// Gets the user's email addresses.
    /// </summary>
    public string[] Emails => this.GetClaim<string[]>(ClaimTypes.Emails);

    /// <summary>
    /// Gets the tenant ID from the request headers.
    /// </summary>
    public Guid Tenant => this.GetHeader<Guid>("X-Tenant");

    /// <summary>
    /// Gets the current user's claims principal.
    /// </summary>
    public System.Security.Claims.ClaimsPrincipal User => httpContextAccessor.HttpContext.User;

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName => this.GetClaim<string>(ClaimTypes.FirstName);

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName => this.GetClaim<string>(ClaimTypes.LastName);

    /// <summary>
    /// Gets the user's city.
    /// </summary>
    public string City => this.GetClaim<string>(ClaimTypes.City);

    /// <summary>
    /// Gets the user's country.
    /// </summary>
    public string Country => this.GetClaim<string>(ClaimTypes.Country);

    /// <summary>
    /// Gets the user's postal code.
    /// </summary>
    public string PostalCode => this.GetClaim<string>(ClaimTypes.PostalCode);

    /// <summary>
    /// Gets the user's street address.
    /// </summary>
    public string StreetAddress => this.GetClaim<string>(ClaimTypes.StreetAddress);

    /// <summary>
    /// Gets the user's state.
    /// </summary>
    public string State => this.GetClaim<string>(ClaimTypes.State);

    /// <summary>
    /// Gets the user's job title.
    /// </summary>
    public string JobTitle => this.GetClaim<string>(ClaimTypes.JobTitle);

    /// <summary>
    /// Gets the value of a specified claim type.
    /// </summary>
    /// <typeparam name="TValue">The type of the claim value.</typeparam>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The claim value.</returns>
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
    /// Gets the value of a specified header.
    /// </summary>
    /// <typeparam name="TValue">The type of the header value.</typeparam>
    /// <param name="header">The header name.</param>
    /// <returns>The header value.</returns>
    public TValue GetHeader<TValue>(string header)
    {
        if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue(header, out var values))
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