namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// WRITE contract for a <see cref="WithholdingDefinition"/> received from a client.
/// <para>
/// <b><see cref="MinimumBase"/> is expressed in MAJOR units</b> (pesos, dollars). The command handler
/// converts it via <see cref="ToValueObject"/> after resolving <c>decimalDigits</c> from
/// <c>ICurrencyGrpc.GetCurrencyAsync(code)</c>.
/// </para>
/// <para>
/// <b><see cref="RatePercentage"/> is expressed as a PERCENTAGE</b> exactly as the user typed it
/// (3.5 for 3.5%). <see cref="ToValueObject"/> converts it to the basis points the value object stores.
/// The client never computes basis points.
/// </para>
/// </summary>
/// <param name="Code">The withholding code (e.g., "RETE_FUENTE"). Normalized to uppercase by the value object.</param>
/// <param name="Name">The human-readable name of the withholding.</param>
/// <param name="RatePercentage">The rate as a percentage (e.g., 3.5 = 3.5%).</param>
/// <param name="MinimumBase">Minimum transaction amount in MAJOR units to apply this withholding. Zero = always applies.</param>
/// <param name="Currency">The 3-letter currency code for the minimum base, according to ISO 4217.</param>
public sealed record WithholdingDefinitionInput(
    string Code,
    string Name,
    decimal RatePercentage,
    decimal MinimumBase,
    string Currency)
{
    /// <summary>
    /// Converts this input into a <see cref="WithholdingDefinition"/> value object,
    /// with the minimum base in minor units and the rate in basis points.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency, from <c>Currency.DecimalDigits</c>.</param>
    /// <returns>A new <see cref="WithholdingDefinition"/> instance.</returns>
    public WithholdingDefinition ToValueObject(short decimalPlaces)
    {
        var minimumBaseMinor = Money.FromDecimal(MinimumBase, Currency, decimalPlaces).Amount;
        var rateBasisPoints = BasisPoints.FromPercentage(RatePercentage);

        return WithholdingDefinition.Create(Code, Name, rateBasisPoints, minimumBaseMinor, Currency);
    }
}
