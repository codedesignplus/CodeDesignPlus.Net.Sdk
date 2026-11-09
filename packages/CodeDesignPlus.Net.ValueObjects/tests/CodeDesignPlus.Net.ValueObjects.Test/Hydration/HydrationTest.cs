using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Common;
using CodeDesignPlus.Net.ValueObjects.Financial;
using CodeDesignPlus.Net.ValueObjects.Location;
using CodeDesignPlus.Net.ValueObjects.Payment;
using CodeDesignPlus.Net.ValueObjects.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using Serializer = CodeDesignPlus.Net.Serializers.JsonSerializer;
using VoFile = CodeDesignPlus.Net.ValueObjects.FileServer.File;
using VoLocation = CodeDesignPlus.Net.ValueObjects.Location.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Hydration;

/// <summary>
/// Reading a value object never validates it; writing it does (pendings/737). Mongo, the cache and the deserialization of
/// events and requests rebuild it through its constructor, and a stored value that would be rejected today (edited by hand,
/// or saved before a rule tightened) used to make the read of the whole aggregate fail with a 500.
/// </summary>
public class HydrationTest
{
    static HydrationTest()
    {
        // The micros store Guids in the standard representation; the driver refuses to serialize them without one.
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }

    /// <summary>
    /// Each value object stored with one invalid field, and the error its write rejects it with.
    /// </summary>
    public static TheoryData<string, Func<object>, string> InvalidStoredValues() => new()
    {
        { nameof(Money), () => Hydrate(Money.FromLong(100, "COP"), j => j["Currency"] = "COPX"), Errors.CurrencyCodeMustBeExactly3CharactersISO.Code },
        { nameof(DateRange), () => Hydrate(DateRange.Create(Start, Start.AddDays(5)), j => j["End"] = Start.AddDays(-5)), Errors.EndMustBeAfterStart.Code },
        { nameof(VoFile), () => Hydrate(ValidFile(), j => j["Size"] = -1), Errors.SizeCannotBeNegative.Code },
        { nameof(Currency), () => Hydrate(ValidCurrency(), j => j["Code"] = "COPX"), Errors.CurrencyCodeLengthIsInvalid.Code },
        { nameof(TaxDefinition), () => Hydrate(ValidTax(), j => j["RateBasisPoints"] = 200000), Errors.RateBasisPointsMustBeBetween0And100000.Code },
        { nameof(WithholdingDefinition), () => Hydrate(WithholdingDefinition.Create("RF", "Retención", 350, 0, "COP", WithholdingBasis.Subtotal), j => j["MinimumBase"] = -1), Errors.MinimumBaseCannotBeNegative.Code },
        { nameof(WithholdingDefinition) + ".Basis", () => Hydrate(WithholdingDefinition.Create("RF", "Retención", 350, 0, "COP", WithholdingBasis.Subtotal), j => j["Basis"] = 7), Errors.WithholdingBasisIsInvalid.Code },
        { nameof(PenaltyRule), () => Hydrate(PenaltyRule.CreateDailyRate(6, "COP"), j => j["Type"] = "WEEKLY_RATE"), Errors.TypeMustBeDAILYRATEFIXEDOrPERCENTAGE.Code },
        { nameof(LineItem), () => Hydrate(ValidLineItem(), j => j["Quantity"] = 0), Errors.QuantityMustBeAtLeast1.Code },
        { nameof(LineItem) + ".Taxes", () => Hydrate(ValidLineItem(), j => j["Taxes"]![0]!["RateBasisPoints"] = 200000), Errors.RateBasisPointsMustBeBetween0And100000.Code },
        { nameof(City), () => Hydrate(ValidCity(), j => j["Name"] = ""), Errors.CityNameCannotBeNullOrEmpty.Code },
        { nameof(Locality), () => Hydrate(Locality.Create(Guid.NewGuid(), "Chapinero"), j => j["Id"] = Guid.Empty), Errors.LocalityIDCannotBeEmpty.Code },
        { nameof(Neighborhood), () => Hydrate(Neighborhood.Create(Guid.NewGuid(), "Chicó"), j => j["Id"] = Guid.Empty), Errors.NeighborhoodIDCannotBeEmpty.Code },
        { nameof(State), () => Hydrate(ValidState(), j => j["Code"] = ""), Errors.StateCodeCannotBeNullOrEmpty.Code },
        { nameof(Country), () => Hydrate(ValidCountry(), j => j["Alpha2"] = "COL"), Errors.CountryAlpha2CodeLengthIsInvalid.Code },
        { nameof(Country) + ".Currency", () => Hydrate(ValidCountry(), j => j["Currency"]!["Code"] = "COPX"), Errors.CurrencyCodeLengthIsInvalid.Code },
        { nameof(VoLocation), () => Hydrate(ValidLocation(), j => j["Address"] = ""), Errors.AddressCannotBeNullOrEmpty.Code },
        { nameof(VoLocation) + ".Country", () => Hydrate(ValidLocation(), j => j["Country"]!["Alpha3"] = "CO"), Errors.CountryAlpha3CodeLengthIsInvalid.Code },
        { nameof(VoLocation) + ".City", () => Hydrate(ValidLocation(), j => j["City"]!["Id"] = Guid.Empty), Errors.CityIDCannotBeEmpty.Code },
        { nameof(CreditCard), () => Hydrate(ValidCreditCard(), j => j["Last4Digits"] = "12"), Errors.CreditCardLast4DigitsMustBeExactly.Code },
        { nameof(Pse), () => Hydrate(ValidPse(), j => j["TypePerson"] = "NJ"), Errors.TypePersonCannotBeGreaterThan1Character.Code },
        { nameof(PaymentMethod), () => Hydrate(PaymentMethod.Create("PSE", ValidPse(), null), j => j["Pse"] = null), Errors.PaymentMethodDetailsCannotBeNull.Code },
        { nameof(PaymentMethod) + ".Pse", () => Hydrate(PaymentMethod.Create("PSE", ValidPse(), null), j => j["Pse"]!["TypePerson"] = "NJ"), Errors.TypePersonCannotBeGreaterThan1Character.Code },
        { nameof(Address), () => Hydrate(ValidAddress(), j => j["Country"] = "Colombia"), Errors.CountryMustBeAValidISO31661.Code },
        { nameof(TypeDocument), () => Hydrate(ValidTypeDocument(), j => j["Code"] = "CCXX"), Errors.CodeCannotBeGreaterThan3Characters.Code },
        { nameof(Buyer), () => Hydrate(ValidBuyer(), j => j["Name"] = ""), Errors.NameCannotBeNullOrEmpty2.Code },
        { nameof(Buyer) + ".TypeDocument", () => Hydrate(ValidBuyer(), j => j["TypeDocument"]!["Code"] = "CCXX"), Errors.CodeCannotBeGreaterThan3Characters.Code },
        { nameof(ContactInfo), () => Hydrate(ContactInfo.Create("+573001234567", "ana@example.com"), j => j["Phone"] = "call me"), Errors.PhoneFormatIsInvalid.Code },
        { nameof(Payer), () => Hydrate(ValidPayer(), j => j["DocumentNumber"] = ""), Errors.DocumentNumberCannotBeNullOrEmpty.Code },
        { nameof(Payer) + ".BillingAddress", () => Hydrate(ValidPayer(), j => j["BillingAddress"]!["Country"] = "Colombia"), Errors.CountryMustBeAValidISO31661.Code },
    };

    [Theory]
    [MemberData(nameof(InvalidStoredValues))]
    public void DeserializeJson_InvalidStoredValue_DoesNotThrow(string valueObject, Func<object> hydrate, string expectedError)
    {
        // Act
        var exception = Record.Exception(hydrate);

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [MemberData(nameof(InvalidStoredValues))]
    public void DeserializeBson_InvalidStoredValue_DoesNotThrow(string valueObject, Func<object> hydrate, string expectedError)
    {
        // Arrange
        var stored = hydrate();
        var document = stored.ToBsonDocument(stored.GetType());

        // Act
        var exception = Record.Exception(() => BsonSerializer.Deserialize(document, stored.GetType()));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [MemberData(nameof(InvalidStoredValues))]
    public void EnsureValid_InvalidStoredValue_ThrowsTheErrorItsWriteRejects(string valueObject, Func<object> hydrate, string expectedError)
    {
        // Arrange
        var stored = (IValidatableValueObject)hydrate();

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(stored.EnsureValid);

        // Assert
        Assert.Equal(expectedError, exception.Code);
    }

    [Fact]
    public void DeserializeJson_InvalidStoredValue_KeepsTheStoredValue()
    {
        // Act
        var money = (Money)Hydrate(Money.FromLong(100, "COP"), j => j["Currency"] = "copx");

        // Assert
        Assert.Equal("COPX", money.Currency);
    }

    [Fact]
    public void FromLong_InvalidCurrency_ThrowsCodeDesignPlusException()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Money.FromLong(100, "COPX"));

        // Assert
        Assert.Equal(Errors.CurrencyCodeMustBeExactly3CharactersISO.Code, exception.Code);
    }

    [Fact]
    public void Create_DateRangeEndBeforeStart_ThrowsCodeDesignPlusException()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DateRange.Create(Start, Start.AddDays(-1)));

        // Assert
        Assert.Equal(Errors.EndMustBeAfterStart.Code, exception.Code);
    }

    [Fact]
    public void Create_LocationWithAnInvalidStoredCountry_ThrowsCodeDesignPlusException()
    {
        // Arrange: a country read from storage with an invalid code is rejected when a new location is written with it.
        var storedCountry = (Country)Hydrate(ValidCountry(), j => j["Alpha2"] = "COL");

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => VoLocation.Create(storedCountry, ValidState(), ValidCity(), null, null, "Calle 1 # 2-3", "110111"));

        // Assert
        Assert.Equal(Errors.CountryAlpha2CodeLengthIsInvalid.Code, exception.Code);
    }

    [Fact]
    public void Create_TypeDocumentCodeLongerThanThree_ThrowsCodeDesignPlusException()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => TypeDocument.Create("CCXX", "Cédula"));

        // Assert
        Assert.Equal(Errors.CodeCannotBeGreaterThan3Characters.Code, exception.Code);
    }

    private static readonly DateTimeOffset Start = new(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);

    private static object Hydrate<T>(T valid, Action<JObject> corrupt)
    {
        var json = JObject.Parse(Serializer.Serialize(valid!));

        corrupt(json);

        return Serializer.Deserialize<T>(json.ToString())!;
    }

    private static VoFile ValidFile() => VoFile.Create(Guid.NewGuid(), "tenants/docs", "acta.pdf", "pdf", "application/pdf", 1024);

    private static Currency ValidCurrency() => Currency.Create(Guid.NewGuid(), "Colombian Peso", "COP", "$", 2, 170);

    private static TaxDefinition ValidTax() => TaxDefinition.Create("IVA", "IVA", 1900, false, "COP");

    private static LineItem ValidLineItem() => LineItem.Create(Guid.NewGuid(), "Parqueadero", 1, 10000, "COP", [ValidTax()]);

    private static City ValidCity() => City.Create(Guid.NewGuid(), "Bogotá", "America/Bogota");

    private static State ValidState() => State.Create(Guid.NewGuid(), "Cundinamarca", "CUN");

    private static Country ValidCountry() => Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", ValidCurrency());

    private static VoLocation ValidLocation() => VoLocation.Create(ValidCountry(), ValidState(), ValidCity(), null, null, "Calle 1 # 2-3", "110111");

    private static CreditCard ValidCreditCard() => CreditCard.Create("tok_123", "4242", "2030/12", "Ana Pérez");

    private static Pse ValidPse() => Pse.Create("1022", "N", "https://kappali.com/pse/return");

    private static Address ValidAddress() => Address.Create("Calle 1 # 2-3", "CO", "Cundinamarca", "Bogotá", "110111");

    private static TypeDocument ValidTypeDocument() => TypeDocument.Create("CC", "Cédula de ciudadanía");

    private static Buyer ValidBuyer() => Buyer.Create(Guid.NewGuid(), "Ana Pérez", "+573001234567", "ana@example.com", ValidTypeDocument(), "1020304050", ValidAddress());

    private static Payer ValidPayer() => Payer.Create("Ana Pérez", "ana@example.com", "+573001234567", ValidTypeDocument(), "1020304050", ValidAddress());
}
