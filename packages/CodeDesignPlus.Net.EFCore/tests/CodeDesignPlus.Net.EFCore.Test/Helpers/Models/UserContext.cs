using CodeDesignPlus.Net.Security.Abstractions;
using System.Security.Claims;

namespace CodeDesignPlus.Net.EFCore.Test.Helpers.Models;

public class UserContext : IUserContext
{
    public bool IsApplication { get; set; }
    public required Guid IdUser { get; set; }
    public Guid Oid { get; set; }
    public bool IsAuthenticated { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? StreetAddress { get; set; }

    public string? State { get; set; }

    public string? JobTitle { get; set; }

    public string[]? Emails { get; set; }

    public Guid Tenant { get; set; }

    public ClaimsPrincipal? User { get; set; }

    public string[] Roles { get; set; } = [];

    public string AccessToken { get; set; } = string.Empty;

    public string UserAgent { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public TValue GetClaim<TValue>(string claimType)
    {
        throw new NotImplementedException();
    }

    public TValue GetHeader<TValue>(string header)
    {
        throw new NotImplementedException();
    }
}
