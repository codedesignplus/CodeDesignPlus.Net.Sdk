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
    public static readonly Error StartCannotBeTheDefaultValue = new("9000", "Start cannot be the default value.");

    public static readonly Error EndCannotBeTheDefaultValue = new("9001", "End cannot be the default value.");

    public static readonly Error EndMustBeAfterStart = new("9002", "End must be after Start.");

    public static readonly Error OtherRangeCannotBeNull = new("9003", "Other range cannot be null.");

    public static readonly Error FileIdCannotBeEmpty = new("9004", "File Id cannot be empty.");

    public static readonly Error TargetCannotBeNullOrEmpty = new("9005", "Target cannot be null or empty.");

    public static readonly Error TargetCannotExceed512Characters = new("9006", "Target cannot exceed 512 characters.");

    public static readonly Error NameCannotBeNullOrEmpty = new("9007", "Name cannot be null or empty.");

    public static readonly Error NameCannotExceed255Characters = new("9008", "Name cannot exceed 255 characters.");

    public static readonly Error ExtensionCannotBeNullOrEmpty = new("9009", "Extension cannot be null or empty.");

    public static readonly Error ExtensionCannotExceed20Characters = new("9010", "Extension cannot exceed 20 characters.");

    public static readonly Error MimeTypeCannotBeNullOrEmpty = new("9011", "MimeType cannot be null or empty.");

    public static readonly Error MimeTypeCannotExceed127Characters = new("9012", "MimeType cannot exceed 127 characters.");

    public static readonly Error SizeCannotBeNegative = new("9013", "Size cannot be negative.");

    public static readonly Error CurrencyIdIsEmpty = new("9014", "CurrencyId is empty");

    public static readonly Error NameIsRequired = new("9015", "Name is required");

    public static readonly Error ProductIdCannotBeEmpty = new("9016", "ProductId cannot be empty.");

    public static readonly Error DescriptionCannotBeNullOrEmpty = new("9017", "Description cannot be null or empty.");

    public static readonly Error QuantityMustBeAtLeast1 = new("9018", "Quantity must be at least 1.");

    public static readonly Error UnitPriceCannotBeNegative = new("9019", "UnitPrice cannot be negative.");

    public static readonly Error CurrencyCannotBeNullOrEmpty = new("9020", "Currency cannot be null or empty.");

    public static readonly Error CurrencyMustBeExactly3CharactersISO4217 = new("9021", "Currency must be exactly 3 characters (ISO 4217).");

    public static readonly Error CurrencyCodeCannotBeNullOrEmpty = new("9022", "Currency code cannot be null or empty.");

    public static readonly Error CurrencyCodeMustBeExactly3CharactersISO = new("9023", "Currency code must be exactly 3 characters (ISO 4217).");

    public static readonly Error DecimalPlacesCannotBeNegative = new("9024", "Decimal places cannot be negative.");

    public static readonly Error CannotSubtractAmountsWithDifferentCurrencies = new("9025", "Cannot subtract amounts with different currencies.");

    public static readonly Error CannotAddAmountsWithDifferentCurrencies = new("9026", "Cannot add amounts with different currencies.");

    public static readonly Error CannotCompareAmountsWithDifferentCurrencies = new("9027", "Cannot compare amounts with different currencies.");

    public static readonly Error TypeCannotBeNullOrEmpty = new("9028", "Type cannot be null or empty.");

    public static readonly Error TypeMustBeDAILYRATEFIXEDOrPERCENTAGE = new("9029", "Type must be DAILY_RATE, FIXED, or PERCENTAGE.");

    public static readonly Error RateBasisPointsCannotBeNegative = new("9030", "RateBasisPoints cannot be negative.");

    public static readonly Error FixedAmountCannotBeNegative = new("9031", "FixedAmount cannot be negative.");

    public static readonly Error GraceDaysCannotBeNegative = new("9032", "GraceDays cannot be negative.");

    public static readonly Error MaxPenaltyAmountCannotBeNegative = new("9033", "MaxPenaltyAmount cannot be negative.");

    public static readonly Error CodeCannotBeNullOrEmpty = new("9034", "Code cannot be null or empty.");

    public static readonly Error RateBasisPointsMustBeBetween0And100000 = new("9035", "RateBasisPoints must be between 0 and 100000.");

    public static readonly Error MinimumBaseCannotBeNegative = new("9036", "MinimumBase cannot be negative.");

    public static readonly Error CityIDCannotBeEmpty = new("9037", "City ID cannot be empty.");

    public static readonly Error CityNameCannotBeNullOrEmpty = new("9038", "City name cannot be null or empty.");

    public static readonly Error CountryIDCannotBeEmpty = new("9039", "Country ID cannot be empty.");

    public static readonly Error CountryNameCannotBeEmpty = new("9040", "Country name cannot be empty.");

    public static readonly Error CountryAlpha2CodeCannotBeEmpty = new("9041", "Country Alpha2 code cannot be empty.");

    public static readonly Error CountryAlpha2CodeLengthIsInvalid = new("9042", "Country Alpha2 code length is invalid.");

    public static readonly Error CountryAlpha3CodeCannotBeEmpty = new("9043", "Country Alpha3 code cannot be empty.");

    public static readonly Error CountryAlpha3CodeLengthIsInvalid = new("9044", "Country Alpha3 code length is invalid.");

    public static readonly Error CountryNumericCodeIsInvalid = new("9045", "Country numeric code is invalid.");

    public static readonly Error CountryPhoneCodeCannotBeEmpty = new("9046", "Country phone code cannot be empty.");

    public static readonly Error CountryTimezoneCannotBeEmpty = new("9047", "Country timezone cannot be empty.");

    public static readonly Error CountryCurrencyIsRequired = new("9048", "Country currency is required.");

    public static readonly Error LocalityIDCannotBeEmpty = new("9049", "Locality ID cannot be empty.");

    public static readonly Error LocalityNameCannotBeNullOrEmpty = new("9050", "Locality name cannot be null or empty.");

    public static readonly Error CountryCannotBeNull = new("9051", "Country cannot be null.");

    public static readonly Error StateCannotBeNull = new("9052", "State cannot be null.");

    public static readonly Error CityCannotBeNull = new("9053", "City cannot be null.");

    public static readonly Error LocalityCannotBeNull = new("9054", "Locality cannot be null.");

    public static readonly Error NeighborhoodCannotBeNull = new("9055", "Neighborhood cannot be null.");

    public static readonly Error AddressCannotBeNullOrEmpty = new("9056", "Address cannot be null or empty.");

    public static readonly Error PostalCodeCannotBeNullOrEmpty = new("9057", "Postal code cannot be null or empty.");

    public static readonly Error NeighborhoodIDCannotBeEmpty = new("9058", "Neighborhood ID cannot be empty.");

    public static readonly Error NeighborhoodNameCannotBeNullOrEmpty = new("9059", "Neighborhood name cannot be null or empty.");

    public static readonly Error StateIDCannotBeEmpty = new("9060", "State ID cannot be empty.");

    public static readonly Error StateNameCannotBeNullOrEmpty = new("9061", "State name cannot be null or empty.");

    public static readonly Error StateCodeCannotBeNullOrEmpty = new("9062", "State code cannot be null or empty.");

    public static readonly Error CreditCardTokenCannotBeNullOrEmpty = new("9063", "Credit Card Token cannot be null or empty");

    public static readonly Error CreditCardLast4DigitsCannotBeNull = new("9064", "Credit Card Last 4 digits cannot be null or empty");

    public static readonly Error CreditCardLast4DigitsMustBeExactly = new("9065", "Credit Card Last 4 digits must be exactly 4 digits");

    public static readonly Error CreditCardExpirationDateCannotBeNullOr = new("9066", "Credit Card Expiration Date cannot be null or empty");

    public static readonly Error CreditCardExpirationDateMustBeInValid = new("9067", "Credit Card Expiration Date must be in valid format");

    public static readonly Error CreditCardHolderNameCannotBeNullOr = new("9068", "Credit Card Holder Name cannot be null or empty");

    public static readonly Error CreditCardSecurityCodeCannotBeNullOr = new("9069", "Credit Card Security Code cannot be null or empty");

    public static readonly Error TypeOfThePaymentMethodCannotBeNull = new("9070", "Type of the payment method cannot be null or empty");

    public static readonly Error PaymentMethodDetailsCannotBeNull = new("9071", "Payment method details cannot be null");

    public static readonly Error OnlyOnePaymentMethodIsAllowed = new("9072", "Only one payment method is allowed");

    public static readonly Error PseCodeCannotBeNullOrEmpty = new("9073", "PseCode cannot be null or empty");

    public static readonly Error PseCodeCannotBeGreaterThan34Characters = new("9074", "PseCode cannot be greater than 34 characters");

    public static readonly Error TypePersonCannotBeNullOrEmpty = new("9075", "TypePerson cannot be null or empty");

    public static readonly Error TypePersonCannotBeGreaterThan1Character = new("9076", "TypePerson cannot be greater than 1 character");

    public static readonly Error PseResponseUrlCannotBeNullOrEmpty = new("9077", "PseResponseUrl cannot be null or empty");

    public static readonly Error PseResponseUrlCannotBeGreaterThan200Characters = new("9078", "PseResponseUrl cannot be greater than 200 characters");

    public static readonly Error PseResponseUrlMustBeAValidFormat = new("9079", "PseResponseUrl must be a valid format");

    public static readonly Error StreetCannotBeNullOrEmpty = new("9080", "Street cannot be null or empty");

    public static readonly Error StreetCannotBeGreaterThan100Characters = new("9081", "Street cannot be greater than 100 characters");

    public static readonly Error CountryCannotBeNullOrEmpty = new("9082", "Country cannot be null or empty");

    public static readonly Error CountryMustBeAValidISO31661 = new("9083", "Country must be a valid ISO 3166-1 Alpha-2 code");

    public static readonly Error CityCannotBeNullOrEmpty = new("9084", "City cannot be null or empty");

    public static readonly Error CityCannotBeGreaterThan50Characters = new("9085", "City cannot be greater than 50 characters");

    public static readonly Error StateCannotBeEmptyWhenProvided = new("9086", "State cannot be empty when provided");

    public static readonly Error StateCannotBeGreaterThan40Characters = new("9087", "State cannot be greater than 40 characters");

    public static readonly Error PostalCodeCannotBeEmptyWhenProvided = new("9088", "Postal code cannot be empty when provided");

    public static readonly Error PostalCodeMustContainOnlyDigits18 = new("9089", "Postal code must contain only digits (1-8 characters)");

    public static readonly Error BuyerIdCannotBeEmpty = new("9090", "BuyerId cannot be empty");

    public static readonly Error NameCannotBeNullOrEmpty2 = new("9091", "Name cannot be null or empty");

    public static readonly Error NameCannotBeGreaterThan124Characters = new("9092", "Name cannot be greater than 124 characters");

    public static readonly Error PhoneCannotBeNullOrEmpty = new("9093", "Phone cannot be null or empty");

    public static readonly Error PhoneContainsInvalidCharacters = new("9094", "Phone contains invalid characters");

    public static readonly Error EmailCannotBeNullOrEmpty = new("9095", "Email cannot be null or empty");

    public static readonly Error EmailContainsInvalidCharacters = new("9096", "Email contains invalid characters");

    public static readonly Error DocumentCannotBeEmptyWhenProvided = new("9097", "Document cannot be empty when provided");

    public static readonly Error DocumentCannotBeGreaterThan20Characters = new("9098", "Document cannot be greater than 20 characters");

    public static readonly Error PhoneCannotBeNullOrEmpty2 = new("9099", "Phone cannot be null or empty.");

    public static readonly Error PhoneFormatIsInvalid = new("9100", "Phone format is invalid.");

    public static readonly Error EmailCannotBeNullOrEmpty2 = new("9101", "Email cannot be null or empty.");

    public static readonly Error EmailFormatIsInvalid = new("9102", "Email format is invalid.");

    public static readonly Error AlternatePhoneCannotBeEmptyWhenProvided = new("9103", "AlternatePhone cannot be empty when provided.");

    public static readonly Error AlternatePhoneFormatIsInvalid = new("9104", "AlternatePhone format is invalid.");

    public static readonly Error AlternateEmailCannotBeEmptyWhenProvided = new("9105", "AlternateEmail cannot be empty when provided.");

    public static readonly Error AlternateEmailFormatIsInvalid = new("9106", "AlternateEmail format is invalid.");

    public static readonly Error FullNameCannotBeNullOrEmpty = new("9107", "Full name cannot be null or empty");

    public static readonly Error FullNameCannotBeGreaterThan150Characters = new("9108", "Full name cannot be greater than 150 characters");

    public static readonly Error DocumentNumberCannotBeNullOrEmpty = new("9109", "Document number cannot be null or empty");

    public static readonly Error DocumentNumberCannotBeGreaterThan20Characters = new("9110", "Document number cannot be greater than 20 characters");

    public static readonly Error EmailAddressCannotBeEmptyWhenProvided = new("9111", "Email address cannot be empty when provided");

    public static readonly Error EmailAddressCannotBeGreaterThan255Characters = new("9112", "Email address cannot be greater than 255 characters");

    public static readonly Error EmailAddressFormatIsInvalid = new("9113", "Email address format is invalid");

    public static readonly Error ContactPhoneCannotBeEmptyWhenProvided = new("9114", "Contact phone cannot be empty when provided");

    public static readonly Error ContactPhoneFormatIsInvalid = new("9115", "Contact phone format is invalid");

    public static readonly Error CodeCannotBeNullOrEmpty2 = new("9116", "Code cannot be null or empty");

    public static readonly Error CodeCannotBeGreaterThan3Characters = new("9117", "Code cannot be greater than 3 characters");

    /// <summary>
    /// Los cinco de <c>Currency</c> llevan el nombre y el codigo de la moneda dentro del mensaje.
    /// </summary>
    /// <remarks>
    /// Eran cadenas interpoladas, que no se pueden traducir: el texto se arma en el sitio y no hay plantilla
    /// que buscar. Ahora son plantillas con <c>{0}</c> y el valor entra por <c>With(...)</c>, que formatea en
    /// el borde contra el idioma que toque.
    /// </remarks>
    public static readonly Error CurrencyCodeIsRequired = new("9118", "Code is required for {0}-{1}");

    public static readonly Error CurrencyCodeLengthIsInvalid = new("9119", "Code length is invalid for {0}-{1}");

    public static readonly Error CurrencySymbolIsRequired = new("9120", "Symbol is required for {0}-{1}");

    public static readonly Error CurrencyNumericCodeIsInvalid = new("9121", "Numeric code is invalid for {0}-{1}");

    public static readonly Error CurrencyDecimalDigitsAreInvalid = new("9122", "Decimal digits is invalid for {0}-{1}");
}
