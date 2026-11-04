using NodaTime;

namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// The status of a license at a given instant, together with the dates it was computed from.
/// </summary>
/// <remarks>
/// It is the only place that decides between active, grace and read-only (Kappali pendings/737): the license
/// middleware and any job that warns about an expiring license must read the same answer, or the screen would say
/// «te quedan 3 días» while the API already refuses to write.
/// </remarks>
/// <param name="Status">The status of the license.</param>
/// <param name="ExpirationDate">The instant the license expires.</param>
/// <param name="GraceEndsAt">The instant the grace period ends and the tenant becomes read-only.</param>
public sealed record LicenseStanding(LicenseStatus Status, Instant ExpirationDate, Instant GraceEndsAt)
{
    /// <summary>
    /// Computes the standing of a license.
    /// </summary>
    /// <remarks>
    /// Both limits are inclusive: the license is active up to its expiration instant, and in grace up to the end of
    /// the grace period. A tenant without license arrives with <see cref="Instant.MaxValue"/> as its expiration date,
    /// so the end of the grace period is clamped instead of overflowing.
    /// </remarks>
    /// <param name="expirationDate">The instant the license expires.</param>
    /// <param name="now">The instant to evaluate.</param>
    /// <param name="graceDays">The days of grace after the expiration date.</param>
    /// <returns>The standing of the license at <paramref name="now"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="graceDays"/> is negative.</exception>
    public static LicenseStanding Evaluate(Instant expirationDate, Instant now, int graceDays)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(graceDays);

        var grace = Duration.FromDays(graceDays);

        var graceEndsAt = Instant.MaxValue - expirationDate < grace ? Instant.MaxValue : expirationDate + grace;

        var status = now <= expirationDate
            ? LicenseStatus.Active
            : now <= graceEndsAt ? LicenseStatus.Grace : LicenseStatus.ReadOnly;

        return new LicenseStanding(status, expirationDate, graceEndsAt);
    }
}
