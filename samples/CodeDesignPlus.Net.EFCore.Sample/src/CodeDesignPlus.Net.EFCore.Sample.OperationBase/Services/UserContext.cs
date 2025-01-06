using System;
using System.Security.Claims;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Sample.OperationBase.Services;

/// <summary>
/// This implementation is only for testing purposes, in a real scenario, you use the implementation of the library CodeDesignPlus.Net.Security
/// </summary>
public class UserContext : IUserContext
{
    public bool IsApplication { get; } = false;

    public Guid IdUser { get; }

    public bool IsAuthenticated { get; }

    public string Name { get; } = "Walter White";

    public string FirstName { get; } = "Walter";    

    public string LastName { get; } = "White";

    public string City { get; } = "New York";

    public string Country { get; } = "United States";

    public string PostalCode { get; } = "87101";

    public string StreetAddress { get; } = "308 Negra Arroyo Lane";

    public string State { get; } = "New York";

    public string JobTitle { get; } = "Chemistry Teacher";

    public string[] Emails { get; } = ["white.walter@outlook.com"];

    public Guid Tenant { get; } = Guid.NewGuid();

    public ClaimsPrincipal User { get; } = new ClaimsPrincipal();

    public TValue GetClaim<TValue>(string claimType)
    {
        throw new NotImplementedException();
    }

    public TValue GetHeader<TValue>(string header)
    {
        throw new NotImplementedException();
    }
}
