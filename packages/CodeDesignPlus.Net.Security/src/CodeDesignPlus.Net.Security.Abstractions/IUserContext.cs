namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Provides information about the authenticated user during the request.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets a value indicating whether the current user is an application.
    /// </summary>
    bool IsApplication { get; }

    /// <summary>
    /// Gets the ID of the authenticated user.
    /// </summary>
    Guid IdUser { get; }

    /// <summary>
    /// Gets a value indicating whether the user has been authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the first name of the current user.
    /// </summary>
    string FirstName { get; }

    /// <summary>
    /// Gets the last name of the current user.
    /// </summary>
    string LastName { get; }

    /// <summary>
    /// Gets the city of the current user.
    /// </summary>
    string City { get; }

    /// <summary>
    /// Gets the country of the current user.
    /// </summary>
    string Country { get; }

    /// <summary>
    /// Gets the postal code of the current user.
    /// </summary>
    string PostalCode { get; }

    /// <summary>
    /// Gets the street address of the current user.
    /// </summary>
    string StreetAddress { get; }

    /// <summary>
    /// Gets the state of the current user.
    /// </summary>
    string State { get; }

    /// <summary>
    /// Gets the job title of the current user.
    /// </summary>
    string JobTitle { get; }

    /// <summary>
    /// Gets the email addresses of the current user.
    /// </summary>
    string[] Emails { get; }

    /// <summary>
    /// Gets the tenant ID of the current user.
    /// </summary>
    Guid Tenant { get; }

    /// <summary>
    /// Gets the claims principal containing the user's claims.
    /// </summary>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Gets the value of a specified claim for the authenticated user.
    /// </summary>
    /// <typeparam name="TValue">The type of the claim value.</typeparam>
    /// <param name="claimType">The type of claim to retrieve.</param>
    /// <returns>The value of the specified claim.</returns>
    TValue GetClaim<TValue>(string claimType);

    /// <summary>
    /// Gets the value of a specified header from the request.
    /// </summary>
    /// <typeparam name="TValue">The type of the header value.</typeparam>
    /// <param name="header">The name of the header to retrieve.</param>
    /// <returns>The value of the specified header.</returns>
    TValue GetHeader<TValue>(string header);
}