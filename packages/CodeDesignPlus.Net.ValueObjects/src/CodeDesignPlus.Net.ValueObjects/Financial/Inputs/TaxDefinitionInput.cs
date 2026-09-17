namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// WRITE contract for a <see cref="TaxDefinition"/> received from a client.
/// <para>
/// <b><see cref="RatePercentage"/> is expressed as a PERCENTAGE</b> exactly as the user typed it
/// (19 for 19%, 1.5 for 1.5%). <see cref="ToValueObject"/> converts it to the basis points the
/// value object stores. The client never computes basis points.
/// </para>
/// <para>
/// <b>There is no minimum base.</b> A tax is charged from the first cent; minimum thresholds belong to
/// withholding and live in <see cref="WithholdingDefinitionInput"/>. This record used to accept one, and a
/// client that sent it got a tax that stopped applying below that amount.
/// </para>
/// </summary>
/// <param name="Code">The tax code (e.g., "IVA", "VAT"). Normalized to uppercase by the value object.</param>
/// <param name="Name">The human-readable name of the tax.</param>
/// <param name="RatePercentage">The rate as a percentage (e.g., 19 = 19%, 1.5 = 1.5%).</param>
/// <param name="IsInclusive">Whether the tax is included in the price or added on top.</param>
/// <param name="Currency">The 3-letter currency code, according to ISO 4217.</param>
public sealed record TaxDefinitionInput(
    string Code,
    string Name,
    decimal RatePercentage,
    bool IsInclusive,
    string Currency)
{
    /// <summary>
    /// Converts this input into a <see cref="TaxDefinition"/> value object, with the rate in basis points.
    /// </summary>
    /// <returns>A new <see cref="TaxDefinition"/> instance.</returns>
    public TaxDefinition ToValueObject()
    {
        var rateBasisPoints = BasisPoints.FromPercentage(RatePercentage);

        return TaxDefinition.Create(Code, Name, rateBasisPoints, IsInclusive, Currency);
    }
}
