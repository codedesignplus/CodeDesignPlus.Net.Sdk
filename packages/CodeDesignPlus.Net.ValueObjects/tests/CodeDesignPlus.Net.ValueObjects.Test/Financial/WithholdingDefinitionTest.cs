using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Financial;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;
using Serializer = CodeDesignPlus.Net.Serializers.JsonSerializer;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

/// <summary>
/// Covers <see cref="WithholdingDefinition"/> and the amount its rate applies to (Kappali pendings/668).
/// </summary>
/// <remarks>
/// A lease of $700,000 plus a VAT of $133,000 withheld 15% of the rent ($105,000) as VAT withholding, when the law asks for
/// 15% of the VAT ($19,950): the definition did not know whether its rate applies to the subtotal or to the tax.
/// </remarks>
public class WithholdingDefinitionTest
{
    private const long Rent = 700_000_00L;

    private const long RentVat = 133_000_00L;

    private static WithholdingDefinition VatWithholding(long minimumBase = 0L) =>
        WithholdingDefinition.Create("RTE_IVA_ARRIENDO_INMUEBLE", "ReteIVA arriendo", 1500, minimumBase, "COP", WithholdingBasis.Tax);

    private static WithholdingDefinition IncomeWithholding(long minimumBase = 0L) =>
        WithholdingDefinition.Create("RTE_FUENTE_ARRIENDO_INMUEBLE", "Retefuente arriendo", 350, minimumBase, "COP", WithholdingBasis.Subtotal);

    [Fact]
    public void CalculateWithholdingAmount_TaxBasis_AppliesTheRateToTheTax()
    {
        // Act
        var amount = VatWithholding().CalculateWithholdingAmount(Rent, RentVat);

        // Assert
        Assert.Equal(19_950_00L, amount);
    }

    [Fact]
    public void CalculateWithholdingAmount_SubtotalBasis_AppliesTheRateToTheSubtotal()
    {
        // Act
        var amount = IncomeWithholding().CalculateWithholdingAmount(Rent, RentVat);

        // Assert
        Assert.Equal(24_500_00L, amount);
    }

    /// <summary>
    /// The minimum base of the VAT withholding is compared against the value without VAT, not against the VAT: a VAT of
    /// $133,000 is under a minimum of $200,000, but the rent of $700,000 is over it.
    /// </summary>
    [Fact]
    public void CalculateWithholdingAmount_TaxBasis_ComparesTheMinimumAgainstTheSubtotal()
    {
        // Arrange
        var withholding = VatWithholding(minimumBase: 200_000_00L);

        // Act
        var amount = withholding.CalculateWithholdingAmount(Rent, RentVat);

        // Assert
        Assert.Equal(19_950_00L, amount);
    }

    [Fact]
    public void CalculateWithholdingAmount_SubtotalUnderTheMinimum_ReturnsZero()
    {
        // Arrange
        var withholding = VatWithholding(minimumBase: Rent + 1);

        // Act
        var amount = withholding.CalculateWithholdingAmount(Rent, RentVat);

        // Assert
        Assert.Equal(0L, amount);
    }

    [Fact]
    public void CalculateWithholdingAmount_TaxBasisWithoutTax_ReturnsZero()
    {
        // Act
        var amount = VatWithholding().CalculateWithholdingAmount(Rent, 0L);

        // Assert
        Assert.Equal(0L, amount);
    }

    [Fact]
    public void CalculateWithholdingAmount_StoredWithoutBasis_ThrowsCodeDesignPlusException()
    {
        // Arrange
        var stored = Serializer.Deserialize<WithholdingDefinition>(WithoutBasis(Serializer.Serialize(IncomeWithholding())))!;

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => stored.CalculateWithholdingAmount(Rent, RentVat));

        // Assert
        Assert.Equal(Errors.WithholdingBasisIsInvalid.Code, exception.Code);
    }

    [Fact]
    public void Create_UnknownBasis_ThrowsCodeDesignPlusException()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => WithholdingDefinition.Create("RF", "Retención", 350, 0L, "COP", (WithholdingBasis)0));

        // Assert
        Assert.Equal(Errors.WithholdingBasisIsInvalid.Code, exception.Code);
    }

    [Fact]
    public void Equals_DifferentBasis_IsFalse()
    {
        // Arrange
        var subtotal = WithholdingDefinition.Create("RF", "Retención", 350, 0L, "COP", WithholdingBasis.Subtotal);
        var tax = WithholdingDefinition.Create("RF", "Retención", 350, 0L, "COP", WithholdingBasis.Tax);

        // Act & Assert
        Assert.NotEqual(subtotal, tax);
    }

    [Fact]
    public void DeserializeJson_Basis_KeepsTheStoredBasis()
    {
        // Act
        var read = Serializer.Deserialize<WithholdingDefinition>(Serializer.Serialize(VatWithholding()))!;

        // Assert
        Assert.Equal(WithholdingBasis.Tax, read.Basis);
    }

    [Fact]
    public void ToValueObject_Input_CarriesTheBasis()
    {
        // Arrange
        var input = new WithholdingDefinitionInput("RTE_IVA_ARRIENDO_INMUEBLE", "ReteIVA arriendo", 15m, 0m, "COP", WithholdingBasis.Tax);

        // Act
        var definition = input.ToValueObject(2);

        // Assert
        Assert.Equal((WithholdingBasis.Tax, 1500), (definition.Basis, definition.RateBasisPoints));
    }

    /// <summary>
    /// A definition stored before the basis existed is still read (rule 31): the basis is a member, not a constructor
    /// argument, and it comes back unset.
    /// </summary>
    [Fact]
    public void DeserializeBson_StoredWithoutBasis_IsStillRead()
    {
        // Arrange
        var document = VatWithholding().ToBsonDocument();
        document.Remove(nameof(WithholdingDefinition.Basis));

        // Act
        var read = BsonSerializer.Deserialize<WithholdingDefinition>(document);

        // Assert
        Assert.Equal(("RTE_IVA_ARRIENDO_INMUEBLE", (WithholdingBasis)0), (read.Code, read.Basis));
    }

    [Fact]
    public void DeserializeBson_Basis_KeepsTheStoredBasis()
    {
        // Arrange
        var document = VatWithholding().ToBsonDocument();

        // Act
        var read = BsonSerializer.Deserialize<WithholdingDefinition>(document);

        // Assert
        Assert.Equal(WithholdingBasis.Tax, read.Basis);
    }

    private static string WithoutBasis(string json)
    {
        var parsed = JObject.Parse(json);
        parsed.Remove(nameof(WithholdingDefinition.Basis));
        return parsed.ToString();
    }
}
