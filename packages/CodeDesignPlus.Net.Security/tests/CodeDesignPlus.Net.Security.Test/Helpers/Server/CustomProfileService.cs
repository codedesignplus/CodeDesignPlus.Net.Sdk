using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace CodeDesignPlus.Net.Security.Test.Helpers.Server;

public class CustomProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = new List<Claim>
        {
            new ("city", "Bogotá"),
            new ("country", "Colombia"),
            new ("given_name", "Jaramillo Jaramillo"),
            new ("family_name", "Lina Marcela"),
            new ("name", "Jaramillo Jaramillo Lina Marcela"),
            new ("postalCode", "111611"),
            new ("streetAddress", "Calle Siempre Viva"),
            new ("state", "Bogotá D.C"),
            new ("jobTitle", "Arquitecto"),
            new ("emails", "codedesignplus@outlook.com"),
            new ("oid", "802b1e5c-6e40-4e01-8095-c735b4c9959e")
        };

        context.IssuedClaims.AddRange(claims);

        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}