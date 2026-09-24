using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.ValueObjects;

/// <summary>
/// Los errores que lanzan los objetos de valor del SDK.
/// </summary>
/// <remarks>
/// Antes eran literales sueltos escritos en cada guard, con diez codigos repartidos entre 138 errores:
/// un <c>000</c> podia ser «moneda vacia», «fecha por defecto» o «id de fichero vacio», asi que el codigo
/// no identificaba nada y ninguno aparecia en <c>/errors</c>, porque no habia catalogo que inspeccionar.
/// <para>
/// La numeracion arranca en <b>9000</b> y no en 1xx/2xx/3xx, que son las capas de un microservicio: al
/// mezclarse los catalogos en un proceso el codigo tiene que seguir senalando a un solo error.
/// </para>
/// </remarks>
public class Errors : IErrorCodes
{
    public static readonly Error StartCannotBeTheDefaultValue = new("9000");

    public static readonly Error EndCannotBeTheDefaultValue = new("9001");

    public static readonly Error EndMustBeAfterStart = new("9002");

    public static readonly Error OtherRangeCannotBeNull = new("9003");

    public static readonly Error FileIdCannotBeEmpty = new("9004");

    public static readonly Error TargetCannotBeNullOrEmpty = new("9005");

    public static readonly Error TargetCannotExceed512Characters = new("9006");

    public static readonly Error NameCannotBeNullOrEmpty = new("9007");

    public static readonly Error NameCannotExceed255Characters = new("9008");

    public static readonly Error ExtensionCannotBeNullOrEmpty = new("9009");

    public static readonly Error ExtensionCannotExceed20Characters = new("9010");

    public static readonly Error MimeTypeCannotBeNullOrEmpty = new("9011");

    public static readonly Error MimeTypeCannotExceed127Characters = new("9012");

    public static readonly Error SizeCannotBeNegative = new("9013");

    public static readonly Error CurrencyIdIsEmpty = new("9014");

    public static readonly Error NameIsRequired = new("9015");

    public static readonly Error ProductIdCannotBeEmpty = new("9016");

    public static readonly Error DescriptionCannotBeNullOrEmpty = new("9017");

    public static readonly Error QuantityMustBeAtLeast1 = new("9018");

    public static readonly Error UnitPriceCannotBeNegative = new("9019");

    public static readonly Error CurrencyCannotBeNullOrEmpty = new("9020");

    public static readonly Error CurrencyMustBeExactly3CharactersISO4217 = new("9021");

    public static readonly Error CurrencyCodeCannotBeNullOrEmpty = new("9022");

    public static readonly Error CurrencyCodeMustBeExactly3CharactersISO = new("9023");

    public static readonly Error DecimalPlacesCannotBeNegative = new("9024");

    public static readonly Error CannotSubtractAmountsWithDifferentCurrencies = new("9025");

    public static readonly Error CannotAddAmountsWithDifferentCurrencies = new("9026");

    public static readonly Error CannotCompareAmountsWithDifferentCurrencies = new("9027");

    public static readonly Error TypeCannotBeNullOrEmpty = new("9028");

    public static readonly Error TypeMustBeDAILYRATEFIXEDOrPERCENTAGE = new("9029");

    public static readonly Error RateBasisPointsCannotBeNegative = new("9030");

    public static readonly Error FixedAmountCannotBeNegative = new("9031");

    public static readonly Error GraceDaysCannotBeNegative = new("9032");

    public static readonly Error MaxPenaltyAmountCannotBeNegative = new("9033");

    public static readonly Error CodeCannotBeNullOrEmpty = new("9034");

    public static readonly Error RateBasisPointsMustBeBetween0And100000 = new("9035");

    public static readonly Error MinimumBaseCannotBeNegative = new("9036");

    public static readonly Error CityIDCannotBeEmpty = new("9037");

    public static readonly Error CityNameCannotBeNullOrEmpty = new("9038");

    public static readonly Error CountryIDCannotBeEmpty = new("9039");

    public static readonly Error CountryNameCannotBeEmpty = new("9040");

    public static readonly Error CountryAlpha2CodeCannotBeEmpty = new("9041");

    public static readonly Error CountryAlpha2CodeLengthIsInvalid = new("9042");

    public static readonly Error CountryAlpha3CodeCannotBeEmpty = new("9043");

    public static readonly Error CountryAlpha3CodeLengthIsInvalid = new("9044");

    public static readonly Error CountryNumericCodeIsInvalid = new("9045");

    public static readonly Error CountryPhoneCodeCannotBeEmpty = new("9046");

    public static readonly Error CountryTimezoneCannotBeEmpty = new("9047");

    public static readonly Error CountryCurrencyIsRequired = new("9048");

    public static readonly Error LocalityIDCannotBeEmpty = new("9049");

    public static readonly Error LocalityNameCannotBeNullOrEmpty = new("9050");

    public static readonly Error CountryCannotBeNull = new("9051");

    public static readonly Error StateCannotBeNull = new("9052");

    public static readonly Error CityCannotBeNull = new("9053");

    public static readonly Error LocalityCannotBeNull = new("9054");

    public static readonly Error NeighborhoodCannotBeNull = new("9055");

    public static readonly Error AddressCannotBeNullOrEmpty = new("9056");

    public static readonly Error PostalCodeCannotBeNullOrEmpty = new("9057");

    public static readonly Error NeighborhoodIDCannotBeEmpty = new("9058");

    public static readonly Error NeighborhoodNameCannotBeNullOrEmpty = new("9059");

    public static readonly Error StateIDCannotBeEmpty = new("9060");

    public static readonly Error StateNameCannotBeNullOrEmpty = new("9061");

    public static readonly Error StateCodeCannotBeNullOrEmpty = new("9062");

    public static readonly Error CreditCardTokenCannotBeNullOrEmpty = new("9063");

    public static readonly Error CreditCardLast4DigitsCannotBeNull = new("9064");

    public static readonly Error CreditCardLast4DigitsMustBeExactly = new("9065");

    public static readonly Error CreditCardExpirationDateCannotBeNullOr = new("9066");

    public static readonly Error CreditCardExpirationDateMustBeInValid = new("9067");

    public static readonly Error CreditCardHolderNameCannotBeNullOr = new("9068");

    public static readonly Error CreditCardSecurityCodeCannotBeNullOr = new("9069");

    public static readonly Error TypeOfThePaymentMethodCannotBeNull = new("9070");

    public static readonly Error PaymentMethodDetailsCannotBeNull = new("9071");

    public static readonly Error OnlyOnePaymentMethodIsAllowed = new("9072");

    public static readonly Error PseCodeCannotBeNullOrEmpty = new("9073");

    public static readonly Error PseCodeCannotBeGreaterThan34Characters = new("9074");

    public static readonly Error TypePersonCannotBeNullOrEmpty = new("9075");

    public static readonly Error TypePersonCannotBeGreaterThan1Character = new("9076");

    public static readonly Error PseResponseUrlCannotBeNullOrEmpty = new("9077");

    public static readonly Error PseResponseUrlCannotBeGreaterThan200Characters = new("9078");

    public static readonly Error PseResponseUrlMustBeAValidFormat = new("9079");

    public static readonly Error StreetCannotBeNullOrEmpty = new("9080");

    public static readonly Error StreetCannotBeGreaterThan100Characters = new("9081");

    public static readonly Error CountryCannotBeNullOrEmpty = new("9082");

    public static readonly Error CountryMustBeAValidISO31661 = new("9083");

    public static readonly Error CityCannotBeNullOrEmpty = new("9084");

    public static readonly Error CityCannotBeGreaterThan50Characters = new("9085");

    public static readonly Error StateCannotBeEmptyWhenProvided = new("9086");

    public static readonly Error StateCannotBeGreaterThan40Characters = new("9087");

    public static readonly Error PostalCodeCannotBeEmptyWhenProvided = new("9088");

    public static readonly Error PostalCodeMustContainOnlyDigits18 = new("9089");

    public static readonly Error BuyerIdCannotBeEmpty = new("9090");

    public static readonly Error NameCannotBeNullOrEmpty2 = new("9091");

    public static readonly Error NameCannotBeGreaterThan124Characters = new("9092");

    public static readonly Error PhoneCannotBeNullOrEmpty = new("9093");

    public static readonly Error PhoneContainsInvalidCharacters = new("9094");

    public static readonly Error EmailCannotBeNullOrEmpty = new("9095");

    public static readonly Error EmailContainsInvalidCharacters = new("9096");

    public static readonly Error DocumentCannotBeEmptyWhenProvided = new("9097");

    public static readonly Error DocumentCannotBeGreaterThan20Characters = new("9098");

    public static readonly Error PhoneCannotBeNullOrEmpty2 = new("9099");

    public static readonly Error PhoneFormatIsInvalid = new("9100");

    public static readonly Error EmailCannotBeNullOrEmpty2 = new("9101");

    public static readonly Error EmailFormatIsInvalid = new("9102");

    public static readonly Error AlternatePhoneCannotBeEmptyWhenProvided = new("9103");

    public static readonly Error AlternatePhoneFormatIsInvalid = new("9104");

    public static readonly Error AlternateEmailCannotBeEmptyWhenProvided = new("9105");

    public static readonly Error AlternateEmailFormatIsInvalid = new("9106");

    public static readonly Error FullNameCannotBeNullOrEmpty = new("9107");

    public static readonly Error FullNameCannotBeGreaterThan150Characters = new("9108");

    public static readonly Error DocumentNumberCannotBeNullOrEmpty = new("9109");

    public static readonly Error DocumentNumberCannotBeGreaterThan20Characters = new("9110");

    public static readonly Error EmailAddressCannotBeEmptyWhenProvided = new("9111");

    public static readonly Error EmailAddressCannotBeGreaterThan255Characters = new("9112");

    public static readonly Error EmailAddressFormatIsInvalid = new("9113");

    public static readonly Error ContactPhoneCannotBeEmptyWhenProvided = new("9114");

    public static readonly Error ContactPhoneFormatIsInvalid = new("9115");

    public static readonly Error CodeCannotBeNullOrEmpty2 = new("9116");

    public static readonly Error CodeCannotBeGreaterThan3Characters = new("9117");

    /// <summary>
    /// Los cinco de <c>Currency</c> llevan el nombre y el codigo de la moneda dentro del mensaje.
    /// </summary>
    /// <remarks>
    /// Eran cadenas interpoladas, que no se pueden traducir: el texto se arma en el sitio y no hay plantilla
    /// que buscar. Ahora son plantillas con <c>{0}</c> y el valor entra por <c>With(...)</c>, que formatea en
    /// el borde contra el idioma que toque.
    /// </remarks>
    public static readonly Error CurrencyCodeIsRequired = new("9118");

    public static readonly Error CurrencyCodeLengthIsInvalid = new("9119");

    public static readonly Error CurrencySymbolIsRequired = new("9120");

    public static readonly Error CurrencyNumericCodeIsInvalid = new("9121");

    public static readonly Error CurrencyDecimalDigitsAreInvalid = new("9122");
}
