using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Commons;

/// <summary>
/// Los errores de validacion de formulario, en el rango <b>9300-9314</b>.
/// </summary>
/// <remarks>
/// Van en el 9xxx, que es el del SDK: 9000 los objetos de valor, 9200 los clientes gRPC y 9300 estos.
/// <para>
/// <b>No van en la centena 4xx</b>, aunque parezca la natural. Los microservicios reparten sus codigos por
/// capa —1xx dominio, 2xx aplicacion, 3xx infraestructura— pero eso es una convencion, no un limite: hay
/// catalogos que pasaron de 399 y siguieron contando. <c>ms-notification</c> usa del 400 al 406, asi que una
/// tabla del SDK en 4xx chocaba con la suya y el microservicio <b>no arrancaba</b>. El 9xxx es el unico
/// rango que solo usa el SDK.
/// </para>
/// <para>
/// <b>Por que hacen falta.</b> FluentValidation trae sus mensajes en varios idiomas y los elige mirando
/// <c>CurrentUICulture</c>, pero los entrypoints se compilan con globalizacion invariante —para no
/// arrastrar ICU al contenedor— y ahi esa cultura no se puede cambiar. Se quedaba en ingles hiciera lo que
/// hiciera el <c>Accept-Language</c>, y el usuario leia «'Name' must not be empty.» debajo de un formulario
/// entero en espanol.
/// </para>
/// <para>
/// Son <b>catorce</b> validadores para las 3.319 reglas del monorepo, y dos de ellos —obligatorio y nulo—
/// son el 81%: no hay que tocar las reglas, basta con traducir catorce plantillas una vez.
/// </para>
/// </remarks>
public class ValidationErrors : IErrorCodes
{
    /// <summary>El campo no puede ir vacio. Cubre <c>NotEmpty</c> y <c>NotNull</c>.</summary>
    public static readonly Error Required = new("9300");

    /// <summary>El texto supera el maximo de caracteres.</summary>
    public static readonly Error TooLong = new("9301");

    /// <summary>El texto se sale del rango de longitud permitido.</summary>
    public static readonly Error LengthOutOfRange = new("9302");

    /// <summary>El valor tiene que ser mayor que otro.</summary>
    public static readonly Error MustBeGreaterThan = new("9303");

    /// <summary>El valor tiene que ser mayor o igual que otro.</summary>
    public static readonly Error MustBeGreaterThanOrEqualTo = new("9304");

    /// <summary>El valor tiene que ser menor que otro.</summary>
    public static readonly Error MustBeLessThan = new("9305");

    /// <summary>El valor tiene que ser menor o igual que otro.</summary>
    public static readonly Error MustBeLessThanOrEqualTo = new("9306");

    /// <summary>El valor no puede ser el indicado.</summary>
    public static readonly Error MustNotEqual = new("9307");

    /// <summary>El valor se sale del intervalo permitido.</summary>
    public static readonly Error OutOfRange = new("9308");

    /// <summary>El valor no esta entre los admitidos.</summary>
    public static readonly Error NotAllowedValue = new("9309");

    /// <summary>El correo no tiene un formato valido.</summary>
    public static readonly Error InvalidEmail = new("9310");

    /// <summary>El valor no sigue el formato esperado.</summary>
    public static readonly Error InvalidFormat = new("9311");

    /// <summary>
    /// El valor no cumple una condicion propia de la regla.
    /// </summary>
    /// <remarks>
    /// Es el cajon de sastre de <c>Must</c>, cuyo mensaje por defecto —«The specified condition was not
    /// met»— no dice nada ni traducido. Esas reglas deberian llevar su propio <c>WithMessage</c> con una
    /// entrada del catalogo del microservicio; hasta que la lleven, al menos dicen que campo fallo.
    /// </remarks>
    public static readonly Error ConditionNotMet = new("9312");

    /// <summary>El titulo del sobre cuando la peticion no pasa la validacion.</summary>
    public static readonly Error ValidationTitle = new("9313");

    /// <summary>El resumen del sobre cuando la peticion no pasa la validacion.</summary>
    public static readonly Error ValidationSummary = new("9314");

    /// <summary>
    /// Traduce el codigo que pone FluentValidation al del catalogo.
    /// </summary>
    /// <remarks>
    /// Un codigo desconocido devuelve <c>null</c> y quien llama se queda con el mensaje original en ingles:
    /// es preferible un texto sin traducir que uno que diga otra cosa. Pasa con los validadores propios y
    /// con los <c>WithErrorCode</c> escritos a mano.
    /// </remarks>
    /// <param name="errorCode">El <c>ErrorCode</c> de FluentValidation, por ejemplo <c>NotEmptyValidator</c>.</param>
    public static Error? FromFluentValidation(string? errorCode) => errorCode switch
    {
        "NotEmptyValidator" or "NotNullValidator" => Required,
        "MaximumLengthValidator" => TooLong,
        "LengthValidator" or "MinimumLengthValidator" or "ExactLengthValidator" => LengthOutOfRange,
        "GreaterThanValidator" => MustBeGreaterThan,
        "GreaterThanOrEqualValidator" => MustBeGreaterThanOrEqualTo,
        "LessThanValidator" => MustBeLessThan,
        "LessThanOrEqualValidator" => MustBeLessThanOrEqualTo,
        "NotEqualValidator" => MustNotEqual,
        "InclusiveBetweenValidator" or "ExclusiveBetweenValidator" => OutOfRange,
        "EnumValidator" => NotAllowedValue,
        "EmailValidator" => InvalidEmail,
        "RegularExpressionValidator" => InvalidFormat,
        "PredicateValidator" or "AsyncPredicateValidator" => ConditionNotMet,
        _ => null,
    };
}
