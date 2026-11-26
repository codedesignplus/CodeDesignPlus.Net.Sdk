using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Da a cada tipo el id de esquema de siempre y desambigua, siempre igual, los nombres que comparten dos tipos.
/// </summary>
/// <remarks>
/// <para>
/// Swashbuckle usa el nombre corto del tipo como id del esquema y, si dos tipos de namespaces distintos se llaman
/// igual, la generación entera falla con «Can't use schemaId … The same schemaId is already used». Pasó al declarar
/// las respuestas de las GET (pendings/753): <c>LicenseDto</c> de <c>License</c> y de <c>Order</c> en ms-licenses, y
/// los dos <c>File</c> de ms-filestorage. El Swagger respondía 500.
/// </para>
/// <para>
/// Antes de dar el primer id se recorren los tipos de todas las operaciones (cuerpos, respuestas y lo que contienen).
/// Un nombre corto que lleva más de un tipo es ambiguo, y <b>todos</b> esos tipos van con el segmento del namespace
/// que los distingue (<c>LicenseLicenseDto</c> y <c>OrderLicenseDto</c>). No depende del orden en que se generan los
/// esquemas: la primera versión le dejaba el nombre corto al primero que lo pedía, y producción y el arnés lo daban
/// a tipos distintos. Los demás tipos conservan el id de Swashbuckle (<c>CityDtoPagination</c> para
/// <c>Pagination&lt;CityDto&gt;</c>).
/// </para>
/// </remarks>
/// <param name="services">Los servicios de la aplicación, para leer las operaciones de los controllers.</param>
public sealed class SchemaIdSelector(IServiceProvider services)
{
    /// <summary>
    /// Segmentos de namespace que no distinguen nada: se saltan al buscar el prefijo.
    /// </summary>
    private static readonly HashSet<string> GenericSegments = new(StringComparer.Ordinal)
    {
        "DataTransferObjects", "Dtos", "Dto", "Models", "Model", "ValueObjects", "Abstractions", "Domain", "Application",
        "Rest", "Contracts", "Entities", "Inputs"
    };

    private readonly Lazy<HashSet<string>> ambiguous = new(() => FindAmbiguousNames(services));

    private readonly ConcurrentDictionary<Type, string> ids = new();

    private readonly ConcurrentDictionary<string, Type> owners = new(StringComparer.Ordinal);

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

        if (this.ambiguous.Value.Contains(ShortName(type)))
            id = Prefix(type) + id;

        // Red de seguridad: un tipo que no salió en el recorrido (un filtro que añade esquemas, por ejemplo) y choca
        // con otro. Se usa el nombre completo para no romper la generación.
        if (this.owners.GetOrAdd(id, type) == type)
            return id;

        var full = (type.FullName ?? type.Name).Replace('.', '_').Replace('+', '_').Replace('`', '_');

        this.owners.TryAdd(full, type);

        return full;
    }

    /// <summary>
    /// El mismo id que daría Swashbuckle: el nombre corto, y en un genérico los argumentos delante.
    /// </summary>
    private string Default(Type type)
    {
        if (!type.IsConstructedGenericType)
            return type.Name;

        var arguments = string.Join("And", type.GetGenericArguments().Select(this.Select));

        return arguments + ShortName(type);
    }

    private static string ShortName(Type type) => type.IsGenericType ? type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)] : type.Name;

    private static string Prefix(Type type)
    {
        var segments = (type.Namespace ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);

        return segments.Reverse().FirstOrDefault(x => !GenericSegments.Contains(x)) ?? string.Empty;
    }

    /// <summary>
    /// Los nombres cortos que llevan dos o más tipos entre los que aparecen en las operaciones.
    /// </summary>
    private static HashSet<string> FindAmbiguousNames(IServiceProvider services)
    {
        var provider = services.GetService<IApiDescriptionGroupCollectionProvider>();

        if (provider is null)
            return [];

        var roots = provider.ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .SelectMany(x => x.ParameterDescriptions.Where(p => p.Source == BindingSource.Body || p.Source == BindingSource.Query).Select(p => p.Type)
                .Concat(x.SupportedResponseTypes.Select(r => r.Type)))
            .Where(x => x is not null);

        var seen = new HashSet<Type>();
        var pending = new Stack<Type>(roots);

        while (pending.Count > 0)
        {
            var next = pending.Pop();
            var type = next is null ? null : Nullable.GetUnderlyingType(next) ?? next;

            if (type is null || !seen.Add(type))
                continue;

            if (type.IsArray)
                pending.Push(type.GetElementType());

            foreach (var argument in type.IsGenericType ? type.GetGenericArguments() : [])
                pending.Push(argument);

            if (IsFramework(type))
                continue;

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                pending.Push(property.PropertyType);
        }

        return seen
            .Where(x => !IsFramework(x) && !x.IsGenericParameter)
            .GroupBy(ShortName, StringComparer.Ordinal)
            .Where(x => x.Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t).Distinct().Count() > 1)
            .Select(x => x.Key)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static bool IsFramework(Type type) =>
        type.IsPrimitive
        || type.Namespace is null
        || type.Namespace.StartsWith("System", StringComparison.Ordinal)
        || type.Namespace.StartsWith("Microsoft", StringComparison.Ordinal)
        || type.Namespace.StartsWith("Newtonsoft", StringComparison.Ordinal)
        || type.Namespace.StartsWith("NodaTime", StringComparison.Ordinal);
}

/// <summary>
/// Conecta <see cref="SchemaIdSelector"/> con SwaggerGen, que necesita los servicios de la aplicación.
/// </summary>
/// <param name="selector">El selector de la aplicación.</param>
internal sealed class ConfigureSchemaIds(SchemaIdSelector selector) : Microsoft.Extensions.Options.IConfigureOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Registra el selector como el que da los ids de esquema.
    /// </summary>
    /// <param name="options">Las opciones de SwaggerGen.</param>
    public void Configure(SwaggerGenOptions options) => options.CustomSchemaIds(selector.Select);
}
