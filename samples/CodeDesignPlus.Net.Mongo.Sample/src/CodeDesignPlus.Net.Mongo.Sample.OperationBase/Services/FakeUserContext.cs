using System;
using System.Security.Claims;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Sample.OperationBase.Services;

public class FakeUserContext : IUserContext
{
    public bool IsApplication => false;

    public Guid IdUser => Guid.Parse("1f4eac7b-4b3b-4b3b-8b3b-4b3b4b3b4b3b");

    public bool IsAuthenticated => true;

    public string Name => "John Doe";

    public string FirstName => "John";

    public string LastName => "Doe";

    public string City => "Bogotá";

    public string Country => "Colombia";

    public string PostalCode => "110111"; 

    public string StreetAddress => "Calle 123 # 123 - 123";

    public string State => "Cundinamarca";

    public string JobTitle => "Software Developer";

    public string[] Emails => ["john.doe@codedesignplus.com"];

    public Guid Tenant => Guid.Parse("2f4eac7b-4b3b-4b3b-8b3b-4b3b4b3b4b3b");

    public ClaimsPrincipal User => new();

    public TValue GetClaim<TValue>(string claimType)
    {
        throw new NotImplementedException();
    }

    public TValue GetHeader<TValue>(string header)
    {
        throw new NotImplementedException();
    }
}
