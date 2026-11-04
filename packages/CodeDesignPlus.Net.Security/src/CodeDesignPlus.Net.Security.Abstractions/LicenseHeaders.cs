namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// The response headers with which the license middleware tells the client how the license of the tenant stands.
/// </summary>
/// <remarks>
/// They are a contract with the frontend, which paints a banner from them (Kappali pendings/737): changing a name or
/// a value silently hides the banner. A browser only lets the script read them if CORS lists them in
/// <c>Access-Control-Expose-Headers</c>.
/// </remarks>
public static class LicenseHeaders
{
    /// <summary>
    /// The header with the status of the license: <see cref="GraceValue"/> or <see cref="ReadOnlyValue"/>. Absent while the license is active.
    /// </summary>
    public const string Status = "X-License-Status";

    /// <summary>
    /// The header with the instant the grace period ends, in ISO-8601 UTC (<c>2026-11-19T05:00:00Z</c>). Only during the grace period.
    /// </summary>
    public const string GraceEnds = "X-License-Grace-Ends";

    /// <summary>
    /// The value of <see cref="Status"/> while the license is in its grace period.
    /// </summary>
    public const string GraceValue = "grace";

    /// <summary>
    /// The value of <see cref="Status"/> once the grace period ended: only reads are allowed.
    /// </summary>
    public const string ReadOnlyValue = "read-only";
}
