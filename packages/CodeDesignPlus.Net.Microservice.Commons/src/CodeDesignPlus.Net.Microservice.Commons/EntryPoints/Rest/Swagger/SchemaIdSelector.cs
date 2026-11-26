using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Da a cada tipo el id de esquema de siempre y solo desambigua cuando dos tipos se llaman igual.
/// </summary>
/// <remarks>
/// Swashbuckle usa el nombre corto del tipo como id del esquema y, si dos tipos de namespaces distintos se llaman
/// igual, la generación entera falla con «Can't use schemaId … The same schemaId is already used». Pasó al declarar
/// las respuestas de las GET (pendings/753): <c>LicenseDto</c> de <c>License</c> y de <c>Order</c> en ms-licenses, y
/// los dos <c>File</c> de ms-filestorage. El Swagger respondía 500.
/// <para>
/// El primer tipo que pide un id se lo queda, con el mismo formato que Swashbuckle (<c>CityDtoPagination</c> para
/// <c>Pagination&lt;CityDto&gt;</c>), así que los ids que ya existían no cambian. El siguiente que lo pide recibe el
/// nombre precedido del segmento del namespace que lo distingue (<c>OrderLicenseDto</c>), o su nombre completo si
/// aun así choca.
/// </para>
/// </remarks>
public sealed class SchemaIdSelector
{
    /// <summary>
    /// Segmentos de namespace que no distinguen nada: se saltan al buscar el prefijo.
    /// </summary>
    private static readonly HashSet<string> GenericSegments = new(StringComparer.Ordinal)
    {
        "DataTransferObjects", "Dtos", "Dto", "Models", "Model", "ValueObjects", "Abstractions", "Domain", "Application",
        "Rest", "Contracts", "Entities", "Inputs"
    };

    private readonly ConcurrentDictionary<string, Type> owners = new(StringComparer.Ordinal);

    private readonly ConcurrentDictionary<Type, string> ids = new();

    /// <summary>
    /// El id del esquema de un tipo; siempre el mismo para el mismo tipo.
    /// </summary>
    /// <param name="type">El tipo.</param>
    /// <returns>El id del esquema.</returns>
    public string Select(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return this.ids.GetOrAdd(type, this.Assign);
    }

    private string Assign(Type type)
    {
        var id = this.Default(type);

        if (this.Claim(id, type))
            return id;

        var qualified = Prefix(type) + id;

        if (this.Claim(qualified, type))
            return qualified;

        var full = (type.FullName ?? type.Name).Replace('.', '_').Replace('+', '_').Replace('`', '_');

        this.Claim(full, type);

        return full;
    }

    private bool Claim(string id, Type type) => this.owners.GetOrAdd(id, type) == type;

    /// <summary>
    /// El mismo id que daría Swashbuckle: el nombre corto, y en un genérico los argumentos delante.
    /// </summary>
    private string Default(Type type)
    {
        if (!type.IsConstructedGenericType)
            return type.Name;

        var name = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
        var arguments = string.Join("And", type.GetGenericArguments().Select(this.Select));

        return arguments + name;
    }

    private static string Prefix(Type type)
    {
        var segments = (type.Namespace ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);

        return segments.Reverse().FirstOrDefault(x => !GenericSegments.Contains(x)) ?? string.Empty;
    }
}
