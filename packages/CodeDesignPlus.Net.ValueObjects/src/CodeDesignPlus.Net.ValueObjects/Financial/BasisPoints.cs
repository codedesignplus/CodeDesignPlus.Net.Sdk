namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Conversion between a human-facing percentage and the basis points used for storage.
/// <para>
/// Rates are stored as basis points (1900 = 19.00%) for the same reason money is stored in minor units:
/// an <see cref="int"/> has no floating-point drift, so 19% is always exactly 19%.
/// </para>
/// <para>
/// The client always sends and receives percentages. Converting to basis points is a backend
/// responsibility on write; converting back for display is a client responsibility on read.
/// </para>
/// </summary>
public static class BasisPoints
{
    /// <summary>
    /// The number of basis points in one percentage point.
    /// </summary>
    private const decimal PerPercent = 100m;

    /// <summary>
    /// Converts a percentage as typed by the user into basis points.
    /// </summary>
    /// <param name="percentage">The rate as a percentage (e.g., 19 = 19%, 1.5 = 1.5%).</param>
    /// <returns>The rate in basis points (e.g., 1900, 150).</returns>
    public static int FromPercentage(decimal percentage)
    {
        return (int)Math.Round(percentage * PerPercent, MidpointRounding.AwayFromZero);
    }
}
