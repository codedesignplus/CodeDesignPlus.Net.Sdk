using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// WRITE contract for a <see cref="PenaltyRule"/> received from a client.
/// <para>
/// <b><see cref="FixedAmount"/> and <see cref="MaxPenaltyAmount"/> are expressed in MAJOR units</b>
/// (pesos, dollars). The command handler converts them via <see cref="ToValueObject"/> after resolving
/// <c>decimalDigits</c> from <c>ICurrencyGrpc.GetCurrencyAsync(code)</c>.
/// </para>
/// <para>
/// <b><see cref="RatePercentage"/> is expressed as a PERCENTAGE</b> exactly as the user typed it
/// (1.5 for 1.5% daily). <see cref="ToValueObject"/> converts it to the basis points the value object
/// stores. The client never computes basis points.
/// </para>
/// </summary>
/// <param name="Type">The penalty type. Valid values are "DAILY_RATE", "FIXED", or "PERCENTAGE".</param>
/// <param name="RatePercentage">The rate as a percentage (e.g., 1.5 = 1.5%). Used for "DAILY_RATE" and "PERCENTAGE".</param>
/// <param name="FixedAmount">The fixed penalty in MAJOR units. Used only when <paramref name="Type"/> is "FIXED".</param>
/// <param name="Currency">The 3-letter currency code, according to ISO 4217.</param>
/// <param name="GraceDays">The number of grace days before the penalty starts accruing.</param>
/// <param name="MaxPenaltyAmount">The penalty cap in MAJOR units. Zero means no cap.</param>
public sealed record PenaltyRuleInput(
    string Type,
    decimal RatePercentage,
    decimal FixedAmount,
    string Currency,
    int GraceDays,
    decimal MaxPenaltyAmount)
{
    /// <summary>
    /// Converts this input into a <see cref="PenaltyRule"/> value object, with the amounts in minor units
    /// and the rate in basis points.
    /// Dispatches to the factory matching <see cref="Type"/>, so only the values relevant to that type are carried over.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency, from <c>Currency.DecimalDigits</c>.</param>
    /// <returns>A new <see cref="PenaltyRule"/> instance.</returns>
    public PenaltyRule ToValueObject(short decimalPlaces)
    {
        var normalizedType = Type?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.IsFalse(
            normalizedType is "DAILY_RATE" or "FIXED" or "PERCENTAGE",
            Exceptions.Layer.None,
            "001 : Type must be DAILY_RATE, FIXED, or PERCENTAGE.");

        var rateBasisPoints = BasisPoints.FromPercentage(RatePercentage);
        var maxPenaltyMinor = Money.FromDecimal(MaxPenaltyAmount, Currency, decimalPlaces).Amount;

        if (normalizedType == "DAILY_RATE")
            return PenaltyRule.CreateDailyRate(rateBasisPoints, Currency, GraceDays, maxPenaltyMinor);

        if (normalizedType == "FIXED")
            return PenaltyRule.CreateFixed(Money.FromDecimal(FixedAmount, Currency, decimalPlaces).Amount, Currency, GraceDays);

        return PenaltyRule.CreatePercentage(rateBasisPoints, Currency, GraceDays, maxPenaltyMinor);
    }
}
