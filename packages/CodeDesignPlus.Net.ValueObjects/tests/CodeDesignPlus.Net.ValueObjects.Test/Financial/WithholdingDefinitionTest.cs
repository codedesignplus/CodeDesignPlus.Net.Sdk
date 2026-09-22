using CodeDesignPlus.Net.ValueObjects.Financial;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

/// <summary>
/// Covers <see cref="WithholdingDefinition"/>, and above all what it deliberately cannot do: work out how
/// much to withhold.
/// </summary>
/// <remarks>
/// This type used to hold the rate, a minimum base in money and a currency, and it computed the amount
/// itself. Every document was then a second source of truth for a figure the law sets, and the minimum base
/// — a threshold Colombian law expresses in UVT — was frozen in pesos, so it silently shrank every January.
/// The rate and the threshold now live in the withholding catalogue and are resolved on the document's date.
/// </remarks>
public class WithholdingDefinitionTest
{
    /// <summary>
    /// The code is the whole operative content, and it is normalised.
    /// </summary>
    [Theory]
    [InlineData("RTE_ARRIENDO_INMUEBLE", "RTE_ARRIENDO_INMUEBLE")]
    [InlineData("  rte_arriendo_inmueble  ", "RTE_ARRIENDO_INMUEBLE")]
    [InlineData("rte_Comisiones", "RTE_COMISIONES")]
    public void TheCodeIsNormalised(string given, string expected)
    {
        Assert.Equal(expected, WithholdingDefinition.Create(given).Code);
    }

    /// <summary>
    /// A definition without a code is not a definition: there is nothing left to resolve against.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ACodeIsRequired(string? code)
    {
        Assert.ThrowsAny<Exception>(() => WithholdingDefinition.Create(code!));
    }

    /// <summary>
    /// The display snapshot is optional, because it decides nothing.
    /// </summary>
    /// <remarks>
    /// A charge configured by a background job has nobody to show anything to. Requiring a name there would
    /// force callers to invent one, and an invented name is exactly what this type stopped storing.
    /// </remarks>
    [Fact]
    public void TheDisplaySnapshotIsOptional()
    {
        var definition = WithholdingDefinition.Create("RTE_COMISIONES");

        Assert.Null(definition.AgreedNameForDisplay);
        Assert.Null(definition.AgreedRateBasisPointsForDisplay);
    }

    /// <summary>
    /// When it is given, the snapshot is kept verbatim so a signed contract stays readable.
    /// </summary>
    [Fact]
    public void TheDisplaySnapshotIsKeptAsGiven()
    {
        var definition = WithholdingDefinition.Create("RTE_ARRIENDO_INMUEBLE", "Retencion arrendamiento de bienes inmuebles", 350);

        Assert.Equal("Retencion arrendamiento de bienes inmuebles", definition.AgreedNameForDisplay);
        Assert.Equal(350, definition.AgreedRateBasisPointsForDisplay);
    }

    /// <summary>
    /// Two charges pointing at the same rule are the same withholding, whatever was displayed when each was
    /// configured.
    /// </summary>
    /// <remarks>
    /// <b>This is the one that must not be weakened.</b> If the snapshot counted towards equality, editing a
    /// rate in the catalogue would make every existing charge compare as changed, and the change-detection
    /// that guards a form would start reporting edits nobody made.
    /// </remarks>
    [Fact]
    public void TheSnapshotDoesNotChangeIdentity()
    {
        var beforeTheLawChanged = WithholdingDefinition.Create("RTE_ARRIENDO_INMUEBLE", "Arrendamiento", 350);
        var afterTheLawChanged = WithholdingDefinition.Create("RTE_ARRIENDO_INMUEBLE", "Arrendamiento de inmuebles", 400);

        Assert.Equal(beforeTheLawChanged, afterTheLawChanged);
        Assert.True(beforeTheLawChanged == afterTheLawChanged);
        Assert.Equal(beforeTheLawChanged.GetHashCode(), afterTheLawChanged.GetHashCode());
    }

    /// <summary>
    /// Different rules are different withholdings.
    /// </summary>
    [Fact]
    public void DifferentRulesAreNotEqual()
    {
        var lease = WithholdingDefinition.Create("RTE_ARRIENDO_INMUEBLE");
        var fees = WithholdingDefinition.Create("RTE_HONORARIOS_JURIDICA");

        Assert.NotEqual(lease, fees);
        Assert.True(lease != fees);
    }

    /// <summary>
    /// A displayed rate outside the basis-point range is rejected, even though it is only shown.
    /// </summary>
    /// <remarks>
    /// It is display only, but it is displayed to somebody who is about to sign. A 3.5 typed where basis
    /// points were expected would show as 0.035%, and nothing downstream would contradict it.
    /// </remarks>
    [Theory]
    [InlineData(-1)]
    [InlineData(100_001)]
    public void ADisplayedRateOutsideTheRangeIsRejected(int rateBasisPoints)
    {
        Assert.ThrowsAny<Exception>(() => WithholdingDefinition.Create("RTE_COMISIONES", "Comisiones", rateBasisPoints));
    }

    /// <summary>
    /// The input contract carries the code and, at most, what was shown.
    /// </summary>
    [Fact]
    public void TheInputCarriesTheCodeAndWhatWasShown()
    {
        var input = new WithholdingDefinitionInput("rte_arriendo_inmueble", "Arrendamiento de inmuebles", 3.5m);

        var definition = input.ToValueObject();

        Assert.Equal("RTE_ARRIENDO_INMUEBLE", definition.Code);
        Assert.Equal("Arrendamiento de inmuebles", definition.AgreedNameForDisplay);
        Assert.Equal(350, definition.AgreedRateBasisPointsForDisplay);
    }

    /// <summary>
    /// A client that sends only the code is enough, which is the shape the forms now use.
    /// </summary>
    [Fact]
    public void TheCodeAloneIsEnough()
    {
        var definition = new WithholdingDefinitionInput("RTE_COMISIONES").ToValueObject();

        Assert.Equal("RTE_COMISIONES", definition.Code);
        Assert.Null(definition.AgreedRateBasisPointsForDisplay);
    }

    /// <summary>
    /// What is written can be read back, display snapshot included.
    /// </summary>
    /// <remarks>
    /// This is the one that guards the shape of the constructor. Only <c>code</c> is a parameter and the two
    /// display fields come back through their members, so a stored document keeps reading even after another
    /// display field is added. Had they been constructor parameters, the serializer would demand every one of
    /// them and nothing already saved would load.
    /// </remarks>
    [Fact]
    public void WhatIsWrittenCanBeReadBack()
    {
        var original = WithholdingDefinition.Create("RTE_ARRIENDO_INMUEBLE", "Arrendamiento de inmuebles", 350);

        var restored = JsonConvert.DeserializeObject<WithholdingDefinition>(JsonConvert.SerializeObject(original))!;

        Assert.Equal(original.Code, restored.Code);
        Assert.Equal(original.AgreedNameForDisplay, restored.AgreedNameForDisplay);
        Assert.Equal(original.AgreedRateBasisPointsForDisplay, restored.AgreedRateBasisPointsForDisplay);
    }

    /// <summary>
    /// A document saved without the display snapshot still loads.
    /// </summary>
    /// <remarks>
    /// Not hypothetical: a charge configured by a background job has nothing to display, so this is the
    /// ordinary shape, not the edge case.
    /// </remarks>
    [Fact]
    public void ADocumentWithoutTheSnapshotStillLoads()
    {
        var restored = JsonConvert.DeserializeObject<WithholdingDefinition>("""{"Code":"RTE_COMISIONES"}""")!;

        Assert.Equal("RTE_COMISIONES", restored.Code);
        Assert.Null(restored.AgreedNameForDisplay);
        Assert.Null(restored.AgreedRateBasisPointsForDisplay);
    }
}
