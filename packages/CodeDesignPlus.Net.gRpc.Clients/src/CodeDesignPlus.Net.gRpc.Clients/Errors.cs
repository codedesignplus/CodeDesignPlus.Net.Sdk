using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.gRpc.Clients;

/// <summary>
/// Los errores que lanzan los clientes gRPC del SDK al leer una respuesta que no cuadra.
/// </summary>
/// <remarks>
/// Eran literales sueltos escritos en cada guard, y los dos servicios usaban los mismos cuatro codigos:
/// un <c>000</c> era «pais no encontrado» en uno y «moneda no encontrada» en el otro. Ademas no aparecian
/// en <c>/errors</c>, porque no habia catalogo que inspeccionar.
/// <para>
/// El SDK numera a partir de 9000 para no pisar las capas de un microservicio, que usan 1xx, 2xx y 3xx.
/// Los objetos de valor ocupan 9000-9199 y estos clientes 9200 en adelante.
/// </para>
/// </remarks>
public class Errors : IErrorCodes
{
    public static readonly Error CountryNotFound = new("9200");

    public static readonly Error InvalidCountryId = new("9201");

    public static readonly Error InvalidCountryCode = new("9202");

    public static readonly Error InvalidCountryCurrencyId = new("9203");

    public static readonly Error CurrencyNotFound = new("9210");

    public static readonly Error InvalidCurrencyId = new("9211");

    public static readonly Error InvalidCurrencyCode = new("9212");

    public static readonly Error InvalidCurrencyNumericCode = new("9213");

    public static readonly Error InvalidCurrencyDecimalDigits = new("9214");
}
