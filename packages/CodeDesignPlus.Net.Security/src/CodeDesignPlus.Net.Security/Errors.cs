using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Security;

/// <summary>
/// Los errores que el paquete de seguridad devuelve al cliente, en el rango <b>9400</b>.
/// </summary>
/// <remarks>
/// El SDK numera a partir de 9000 para no pisar las capas de un microservicio (1xx, 2xx, 3xx): 9000 los objetos de
/// valor, 9200 los clientes gRPC, 9300 la validacion de formularios y 9400 este paquete.
/// <para>
/// Nacen del <c>X-Tenant</c> que manda el cliente. Antes, un tenant inexistente respondia 503 —como si el servicio
/// estuviera caido— y encima disparaba una recursion de ms-tenants sobre si mismo (pendings/028). Una cabecera que no
/// era un GUID se ignoraba en silencio y la peticion seguia sin tenant. Las dos son peticiones mal hechas: 400 con su
/// mensaje en el idioma de la peticion.
/// </para>
/// </remarks>
public class Errors : IErrorCodes
{
    /// <summary>La cabecera <c>X-Tenant</c> no es un GUID.</summary>
    public static readonly Error InvalidTenantHeader = new("9400");

    /// <summary>El tenant de la cabecera <c>X-Tenant</c> no existe.</summary>
    public static readonly Error TenantNotFound = new("9401");
}
