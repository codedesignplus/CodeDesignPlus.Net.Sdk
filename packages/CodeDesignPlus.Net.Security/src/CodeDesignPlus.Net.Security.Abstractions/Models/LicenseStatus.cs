namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Where the license of a tenant stands at a given instant.
/// </summary>
/// <remarks>
/// A closed state machine computed from the expiration date and the grace period, not a catalog: nobody adds a fourth
/// state from a screen, and every value changes what the license middleware lets through.
/// </remarks>
public enum LicenseStatus
{
    /// <summary>
    /// The license is in force: the instant is on or before its expiration date.
    /// </summary>
    Active,

    /// <summary>
    /// The license expired but the grace period has not ended: everything still works.
    /// </summary>
    Grace,

    /// <summary>
    /// The grace period ended without a renewal: the tenant can read but not write.
    /// </summary>
    ReadOnly
}
