using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

/// <summary>
/// Covers <see cref="TaxDefinition"/>, and above all the rule that has no exceptions: a tax is charged from
/// the first cent.
/// </summary>
/// <remarks>
/// This type used to carry a minimum base, purely by symmetry with withholding, and callers honoured it. A
/// parking rate of 230 per minute with a minimum base of 8,000 left every stay under 35 minutes untaxed and
/// taxed the one at 35. The same service taxed or not depending on how long it lasted is what a tax audit
/// flags. Minimum thresholds exist for withholding —2 UVT for services, 10 UVT for purchases in Colombia—
/// and they live in <see cref="WithholdingDefinition"/>.
/// </remarks>
public class TaxDefinitionTest
{
    private static readonly TaxDefinition Vat = TaxDefinition.Create("IVA", "Impuesto al Valor Agregado", 1900, false, "COP");

    /// <summary>
    /// A tax applies from the very first cent, with no threshold below which it disappears.
    /// </summary>
    /// <remarks>
    /// <b>This is the one that must never be weakened.</b> One cent is 19% of nothing once rounded, but the
    /// point is the shape of the answer: the tax is computed for every amount, not skipped for small ones.
    /// </remarks>
    [Theory]
    [InlineData(1L, 0L)]
    [InlineData(100L, 19L)]
    [InlineData(23_000L, 4_370L)]
    [InlineData(799_900L, 151_981L)]
    [InlineData(800_000L, 152_000L)]
    public void TheTaxIsChargedFromTheFirstCent(long baseAmount, long expected)
    {
        Assert.Equal(expected, Vat.CalculateTaxAmount(baseAmount));
    }

    /// <summary>
    /// Two stays either side of what used to be a threshold are taxed at the same rate.
    /// </summary>
    /// <remarks>
    /// The figures are the ones from the report: 230 per minute, an 8,000 threshold. Thirty-four minutes came
    /// out untaxed and thirty-five taxed. Now the ratio is identical on both sides.
    /// </remarks>
    [Fact]
    public void TwoStaysEitherSideOfTheOldThresholdAreTaxedAlike()
    {
        var shortStay = 34 * 23_000L;
        var longStay = 35 * 23_000L;

        Assert.True(shortStay < 800_000L);
        Assert.True(longStay >= 800_000L);

        Assert.Equal(148_580L, Vat.CalculateTaxAmount(shortStay));
        Assert.Equal(152_950L, Vat.CalculateTaxAmount(longStay));
    }

    /// <summary>The rate travels in basis points and comes back as a decimal factor.</summary>
    [Fact]
    public void TheRateIsStoredInBasisPoints()
    {
        Assert.Equal(1900, Vat.RateBasisPoints);
        Assert.Equal(0.19m, Vat.ToDecimalRate());
    }

    /// <summary>The code and the currency are normalized; the name is kept as typed.</summary>
    [Fact]
    public void TheCodeAndCurrencyAreNormalized()
    {
        var tax = TaxDefinition.Create(" iva ", "Impuesto al Valor Agregado", 1900, false, "cop");

        Assert.Equal("IVA", tax.Code);
        Assert.Equal("COP", tax.Currency);
        Assert.Equal("Impuesto al Valor Agregado", tax.Name);
    }

    /// <summary>Two definitions with the same data are the same value.</summary>
    [Fact]
    public void TwoDefinitionsWithTheSameDataAreEqual()
    {
        var other = TaxDefinition.Create("IVA", "Impuesto al Valor Agregado", 1900, false, "COP");

        Assert.Equal(Vat, other);
        Assert.True(Vat == other);
        Assert.Equal(Vat.GetHashCode(), other.GetHashCode());
    }

    /// <summary>And two with different rates are not.</summary>
    [Fact]
    public void TwoDefinitionsWithDifferentRatesAreNotEqual()
    {
        var reduced = TaxDefinition.Create("IVA", "Impuesto al Valor Agregado", 500, false, "COP");

        Assert.NotEqual(Vat, reduced);
        Assert.True(Vat != reduced);
    }

    /// <summary>The input converts a percentage typed by a person into basis points.</summary>
    /// <remarks>
    /// The client never computes basis points, and no longer sends a minimum base either: there is nothing
    /// to convert into minor units, so the conversion stopped needing the currency's decimal digits.
    /// </remarks>
    [Fact]
    public void TheInputTurnsAPercentageIntoBasisPoints()
    {
        var input = new TaxDefinitionInput("IVA", "Impuesto al Valor Agregado", 19m, false, "COP");

        var tax = input.ToValueObject();

        Assert.Equal(1900, tax.RateBasisPoints);
        Assert.Equal("IVA", tax.Code);
    }
}
