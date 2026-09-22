namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// WRITE contract for a <see cref="WithholdingDefinition"/> received from a client.
/// <para>
/// <b>The client says which withholding, not how much.</b> <see cref="Code"/> is the only field that decides
/// anything: the rate and the threshold are resolved against the withholding catalogue by the service that
/// issues the document, on the document's own date.
/// </para>
/// <para>
/// This contract used to take the rate, a minimum base in major units and a currency, and it needed the
/// currency's decimal digits to convert. It no longer converts anything, so it no longer needs them.
/// </para>
/// </summary>
/// <param name="Code">The catalogue code (e.g., "RTE_ARRIENDO_INMUEBLE"). Normalized to uppercase by the value object.</param>
/// <param name="Name">What the client displayed when it was picked. Optional, <b>display only</b>.</param>
/// <param name="RatePercentage">The rate the client displayed, as a percentage (3.5 = 3.5%). Optional, <b>display only — never the rate that is applied</b>.</param>
public sealed record WithholdingDefinitionInput(
    string Code,
    string? Name = null,
    decimal? RatePercentage = null)
{
    /// <summary>
    /// Converts this input into a <see cref="WithholdingDefinition"/> value object.
    /// </summary>
    /// <returns>A new <see cref="WithholdingDefinition"/> instance.</returns>
    /// <remarks>
    /// The <c>decimalPlaces</c> parameter is gone, and that is not an oversight: every call site is now a
    /// compile error, which is the point. Silently accepting the old argument would let a handler keep
    /// believing it was storing a rate that mattered.
    /// </remarks>
    public WithholdingDefinition ToValueObject()
    {
        var agreedRateBasisPoints = RatePercentage.HasValue
            ? BasisPoints.FromPercentage(RatePercentage.Value)
            : (int?)null;

        return WithholdingDefinition.Create(Code, Name, agreedRateBasisPoints);
    }
}
