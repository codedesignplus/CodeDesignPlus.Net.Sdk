using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a penalty rule for late payments, damages, or non-compliance.
/// Supports three penalty types: a daily percentage rate, a fixed amount, or a percentage of the original amount.
/// Amounts are stored in minor units (e.g., cents) to avoid floating-point precision loss.
/// </summary>
public sealed class PenaltyRule : IEquatable<PenaltyRule>
{
    /// <summary>
    /// Gets the penalty type. Valid values are "DAILY_RATE", "FIXED", or "PERCENTAGE".
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Gets the penalty rate in basis points.
    /// Used when <see cref="Type"/> is "DAILY_RATE" (per day) or "PERCENTAGE" (of original amount).
    /// e.g., 150 = 1.50% per day, 1000 = 10.00% of original. Zero when Type is "FIXED".
    /// </summary>
    public int RateBasisPoints { get; private set; }

    /// <summary>
    /// Gets the fixed penalty amount in minor units.
    /// Used only when <see cref="Type"/> is "FIXED". Zero otherwise.
    /// </summary>
    public long FixedAmount { get; private set; }

    /// <summary>
    /// Gets the ISO 4217 currency code for the fixed amount (e.g., "COP", "MXN").
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Gets the number of grace days before the penalty starts accruing.
    /// </summary>
    public int GraceDays { get; private set; }

    /// <summary>
    /// Gets the maximum penalty cap in minor units.
    /// Zero means no cap — apply legal limits externally per jurisdiction.
    /// </summary>
    public long MaxPenaltyAmount { get; private set; }

    [JsonConstructor]
    private PenaltyRule(string type, int rateBasisPoints, long fixedAmount, string currency, int graceDays, long maxPenaltyAmount)
    {
        var normalizedType = type?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedType, Exceptions.Layer.None, "000 : Type cannot be null or empty.");
        Guard.IsFalse(
            normalizedType is "DAILY_RATE" or "FIXED" or "PERCENTAGE",
            Exceptions.Layer.None,
            "001 : Type must be DAILY_RATE, FIXED, or PERCENTAGE.");
        Guard.IsNullOrEmpty(normalizedCurrency, Exceptions.Layer.None, "002 : Currency cannot be null or empty.");
        Guard.IsFalse(normalizedCurrency.Length == 3, Exceptions.Layer.None, "003 : Currency must be exactly 3 characters (ISO 4217).");
        Guard.IsLessThan(rateBasisPoints, 0, Exceptions.Layer.None, "004 : RateBasisPoints cannot be negative.");
        Guard.IsLessThan(fixedAmount, 0L, Exceptions.Layer.None, "005 : FixedAmount cannot be negative.");
        Guard.IsLessThan(graceDays, 0, Exceptions.Layer.None, "006 : GraceDays cannot be negative.");
        Guard.IsLessThan(maxPenaltyAmount, 0L, Exceptions.Layer.None, "007 : MaxPenaltyAmount cannot be negative.");

        Type = normalizedType;
        RateBasisPoints = rateBasisPoints;
        FixedAmount = fixedAmount;
        Currency = normalizedCurrency;
        GraceDays = graceDays;
        MaxPenaltyAmount = maxPenaltyAmount;
    }

    /// <summary>
    /// Creates a daily-rate penalty rule (e.g., 1.50% per day after 5 grace days).
    /// </summary>
    /// <param name="rateBasisPoints">The daily rate in basis points (e.g., 150 = 1.50%).</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <param name="graceDays">Number of days before the penalty starts accruing. Defaults to 0.</param>
    /// <param name="maxPenaltyAmount">Maximum penalty cap in minor units. Zero means no cap. Defaults to 0.</param>
    /// <returns>A new <see cref="PenaltyRule"/> instance of type "DAILY_RATE".</returns>
    public static PenaltyRule CreateDailyRate(int rateBasisPoints, string currency, int graceDays = 0, long maxPenaltyAmount = 0)
        => new("DAILY_RATE", rateBasisPoints, 0, currency, graceDays, maxPenaltyAmount);

    /// <summary>
    /// Creates a fixed-amount penalty rule.
    /// </summary>
    /// <param name="fixedAmount">The fixed penalty amount in minor units.</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <param name="graceDays">Number of days before the penalty applies. Defaults to 0.</param>
    /// <returns>A new <see cref="PenaltyRule"/> instance of type "FIXED".</returns>
    public static PenaltyRule CreateFixed(long fixedAmount, string currency, int graceDays = 0)
        => new("FIXED", 0, fixedAmount, currency, graceDays, 0);

    /// <summary>
    /// Creates a percentage-of-original penalty rule (e.g., 10% of the unpaid amount).
    /// </summary>
    /// <param name="rateBasisPoints">The percentage rate in basis points (e.g., 1000 = 10.00%).</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <param name="graceDays">Number of days before the penalty applies. Defaults to 0.</param>
    /// <param name="maxPenaltyAmount">Maximum penalty cap in minor units. Zero means no cap. Defaults to 0.</param>
    /// <returns>A new <see cref="PenaltyRule"/> instance of type "PERCENTAGE".</returns>
    public static PenaltyRule CreatePercentage(int rateBasisPoints, string currency, int graceDays = 0, long maxPenaltyAmount = 0)
        => new("PERCENTAGE", rateBasisPoints, 0, currency, graceDays, maxPenaltyAmount);

    /// <summary>
    /// Calculates the penalty amount for a given overdue base and number of overdue days.
    /// Respects grace days and the maximum cap.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The overdue base amount in minor units.</param>
    /// <param name="overdueDays">Total days past the due date.</param>
    /// <returns>The calculated penalty in minor units, capped by <see cref="MaxPenaltyAmount"/> if set.</returns>
    public long Calculate(long baseAmountMinorUnits, int overdueDays)
    {
        var billableDays = Math.Max(0, overdueDays - GraceDays);
        if (billableDays == 0) return 0L;

        long penalty = Type switch
        {
            "DAILY_RATE" => (long)Math.Round(baseAmountMinorUnits * (RateBasisPoints / 10000m) * billableDays, MidpointRounding.AwayFromZero),
            "FIXED"      => FixedAmount,
            "PERCENTAGE" => (long)Math.Round(baseAmountMinorUnits * (RateBasisPoints / 10000m), MidpointRounding.AwayFromZero),
            _            => 0L
        };

        return MaxPenaltyAmount > 0 ? Math.Min(penalty, MaxPenaltyAmount) : penalty;
    }

    /// <summary>
    /// Returns true if two <see cref="PenaltyRule"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PenaltyRule? a, PenaltyRule? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="PenaltyRule"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PenaltyRule? a, PenaltyRule? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="PenaltyRule"/>.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(PenaltyRule? other)
    {
        if (other is null) return false;
        return Type == other.Type &&
               RateBasisPoints == other.RateBasisPoints &&
               FixedAmount == other.FixedAmount &&
               Currency == other.Currency &&
               GraceDays == other.GraceDays &&
               MaxPenaltyAmount == other.MaxPenaltyAmount;
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is PenaltyRule other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Type, RateBasisPoints, FixedAmount, Currency, GraceDays, MaxPenaltyAmount);
}
