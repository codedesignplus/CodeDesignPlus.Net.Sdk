namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Every role of a user, as published by ms-users.
/// </summary>
/// <remarks>
/// A role is identified by the group id of the identity provider, never by its name: that is what the
/// token carries in the <c>groups</c> claim and what the notification audience stores.
/// <para>
/// The snapshot holds the whole map and not the roles of a single tenant, so that a user switching
/// tenant within a session does not cause one lookup per switch.
/// </para>
/// </remarks>
public class UserRoles
{
    /// <summary>
    /// The user the snapshot belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Roles that do not hang from any tenant and therefore apply everywhere, such as the one that
    /// administers the platform itself.
    /// </summary>
    public string[] Platform { get; set; } = [];

    /// <summary>
    /// Roles per tenant, keyed by the tenant identifier.
    /// </summary>
    public Dictionary<string, string[]> Tenants { get; set; } = [];

    /// <summary>
    /// The effective roles of the user in a tenant: the platform ones plus the ones of that tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>The effective roles, never <c>null</c>.</returns>
    public string[] EffectiveIn(Guid tenantId)
    {
        // La clave se compara parseada y no como texto: la instantanea viaja serializada en JSON y ahi
        // el formato del identificador depende de quien la escribio. Comparar cadenas dejaria a un
        // usuario sin roles por una diferencia de mayusculas, en silencio y solo en algunos entornos.
        var key = this.Tenants.Keys.FirstOrDefault(x => Guid.TryParse(x, out var id) && id == tenantId);

        if (key is null)
            return this.Platform;

        return [.. this.Platform, .. this.Tenants[key]];
    }
}
