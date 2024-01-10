using System.Security.Claims;

namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Provide the information of the authenticated user during the request
/// </summary>
/// <typeparam name="TKeyUser">Type of data that the user will identify</typeparam>
/// <typeparam name="TTenant">Type of data that the tenant will identify</typeparam>
public interface IUserContext<TKeyUser, TTenant>
{
    /// <summary>
    /// Gets a value boolean that indicates whether is a application
    /// </summary>
    bool IsApplication { get; }
    /// <summary>
    /// Gets the Id User authenticated
    /// </summary>
    TKeyUser IdUser { get; }
    /// <summary>
    /// Gets a value that indicates whether the user has been authenticated.
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
    /// Gets the phone number of the current user.
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
    /// Gets the user's job title.
    /// </summary>
    string JobTitle { get; }
    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    string[] Emails { get; }
    /// <summary>
    /// Get or set the tenant user
    /// </summary>
    TTenant Tenant { get; }
    /// <summary>
    /// Gets the claims-principal with the user information
    /// </summary>
    ClaimsPrincipal User { get; }    
    /// <summary>
    /// Gets the claim value of the authenticated user
    /// </summary>
    /// <typeparam name="TValue">Type of data that the claim will return</typeparam>
    /// <param name="claimType">Type of claim to get</param>
    /// <returns>Claim value</returns>
    TValue GetClaim<TValue>(string claimType);
    /// <summary>
    /// Gets the header value of the response
    /// </summary>
    /// <typeparam name="TValue">Type of data that the header will return</typeparam>
    /// <param name="header">Name of the header to get</param>
    /// <returns>Header value</returns>
    TValue GetHeader<TValue>(string header);
}