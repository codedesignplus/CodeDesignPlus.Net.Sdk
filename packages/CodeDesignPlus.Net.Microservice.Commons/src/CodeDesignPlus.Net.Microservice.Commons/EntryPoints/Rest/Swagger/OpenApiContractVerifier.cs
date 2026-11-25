using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Qué tipo de diferencia hay entre el esquema OpenAPI y lo que Newtonsoft serializa.
/// </summary>
public enum ContractMismatchKind
{
    /// <summary>
    /// Newtonsoft escribe la propiedad y el esquema no la tiene.
    /// </summary>
    MissingInSchema,

    /// <summary>
    /// El esquema tiene la propiedad y Newtonsoft no la escribe.
    /// </summary>
    NotSerialized,

    /// <summary>
    /// El body se puede mandar con la propiedad y el esquema la marca de solo lectura.
    /// </summary>
    ReadOnlyButWritable,

    /// <summary>
    /// El esquema deja mandar la propiedad y Newtonsoft la ignora al deserializar.
    /// </summary>
    WritableButIgnored,

    /// <summary>
    /// Un convertidor escribe el tipo como valor simple y el esquema no lo describe así: lo describe como objeto o
    /// lo deja sin tipo (<c>{}</c>, que es lo que hace el soporte de Newtonsoft de Swashbuckle).
    /// </summary>
    ConverterNotDescribedAsValue,

    /// <summary>
    /// El ejemplo del esquema no se puede deserializar al tipo.
    /// </summary>
    InvalidExample
}

/// <summary>
/// Una diferencia entre el esquema OpenAPI de un tipo y su contrato de Newtonsoft.
/// </summary>
/// <param name="Type">El tipo de .NET.</param>
/// <param name="Property">La propiedad, con el nombre del cable; vacía si la diferencia es del tipo entero.</param>
/// <param name="Kind">Qué diferencia es.</param>
/// <param name="Detail">La explicación para quien lee la prueba.</param>
public sealed record ContractMismatch(Type Type, string Property, ContractMismatchKind Kind, string Detail)
{
    /// <summary>
    /// La diferencia en una línea, para el mensaje de la prueba.
    /// </summary>
    public override string ToString() => $"{this.Type.FullName}.{this.Property}: {this.Kind} — {this.Detail}";
}

/// <summary>
/// Comprueba que el Swagger de un micro describe el contrato que de verdad serializa Newtonsoft.
/// </summary>
/// <remarks>
/// El Swagger es el contrato con que un cliente o un gateway MCP llama al micro (pendings/753 y 754). Esta
/// verificación recorre los tipos de cada body y de cada respuesta de los controllers, con la configuración de
/// Newtonsoft de MVC del propio micro, y los compara con el esquema que genera SwaggerGen. Sirve en las pruebas de
/// integración de cualquier micro, sobre su servidor de pruebas, y falla el día que alguien añade un tipo con una
/// forma que el esquema no describe.
/// </remarks>
public static class OpenApiContractVerifier
{
    /// <summary>
    /// Verifica todas las operaciones de los controllers registrados.
    /// </summary>
    /// <param name="services">Los servicios de la aplicación, con MVC, Newtonsoft y SwaggerGen registrados.</param>
    /// <returns>Las diferencias encontradas; vacía si el contrato es fiel.</returns>
    public static IReadOnlyList<ContractMismatch> Verify(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptions = services.GetRequiredService<IApiDescriptionGroupCollectionProvider>()
            .ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .ToList();

        var inputs = descriptions
            .SelectMany(x => x.ParameterDescriptions)
            .Where(x => x.Source == BindingSource.Body && x.Type is not null)
            .Select(x => x.Type);

        var outputs = descriptions
            .SelectMany(x => x.SupportedResponseTypes)
            .Where(x => x.Type is not null && x.Type != typeof(void))
            .Select(x => x.Type);

        return Verify(services, inputs, outputs);
    }

    /// <summary>
    /// Verifica unos tipos de entrada y de salida concretos, con todo lo que contienen.
    /// </summary>
    /// <param name="services">Los servicios de la aplicación, con MVC, Newtonsoft y SwaggerGen registrados.</param>
    /// <param name="inputs">Los tipos que llegan en un body.</param>
    /// <param name="outputs">Los tipos que salen en una respuesta.</param>
    /// <returns>Las diferencias encontradas; vacía si el contrato es fiel.</returns>
    public static IReadOnlyList<ContractMismatch> Verify(IServiceProvider services, IEnumerable<Type> inputs, IEnumerable<Type> outputs)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(outputs);

        var settings = services.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;
        var resolver = settings.ContractResolver ?? new DefaultContractResolver();
        var generator = services.GetRequiredService<ISchemaGenerator>();
        var repository = new SchemaRepository();

        var walker = new Walker(settings, resolver, generator, repository);

        foreach (var type in inputs.Distinct())
            walker.Visit(type, isInput: true);

        foreach (var type in outputs.Distinct())
            walker.Visit(type, isInput: false);

        return walker.Mismatches;
    }

    /// <summary>
    /// Recorre un tipo y los que contiene, una vez por tipo y por dirección.
    /// </summary>
    private sealed class Walker(JsonSerializerSettings settings, IContractResolver resolver, ISchemaGenerator generator, SchemaRepository repository)
    {
        private readonly HashSet<(Type, bool)> visited = [];

        public List<ContractMismatch> Mismatches { get; } = [];

        public void Visit(Type type, bool isInput)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (IsFramework(type) || !this.visited.Add((type, isInput)))
                return;

            var schema = this.Resolve(generator.GenerateSchema(type, repository));

            if (this.HasConverter(type))
            {
                this.VerifyConverted(type, schema);

                return;
            }

            switch (resolver.ResolveContract(type))
            {
                case JsonArrayContract array when array.CollectionItemType is not null:
                    this.Visit(array.CollectionItemType, isInput);
                    break;

                case JsonDictionaryContract dictionary when dictionary.DictionaryValueType is not null:
                    this.Visit(dictionary.DictionaryValueType, isInput);
                    break;

                case JsonObjectContract contract when contract.ExtensionDataSetter is null && contract.ExtensionDataGetter is null:
                    this.VerifyObject(type, contract, schema, isInput);
                    break;
            }
        }

        private void VerifyObject(Type type, JsonObjectContract contract, OpenApiSchema schema, bool isInput)
        {
            var properties = schema?.Properties ?? new Dictionary<string, OpenApiSchema>();
            var creator = contract.CreatorParameters.Select(x => x.PropertyName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var serialized = contract.Properties.Where(x => !x.Ignored && x.Readable).ToList();

            foreach (var property in serialized.Where(x => !properties.ContainsKey(x.PropertyName)))
                this.Add(type, property.PropertyName, ContractMismatchKind.MissingInSchema, "Newtonsoft writes it and the schema does not describe it.");

            foreach (var name in properties.Keys.Where(name => !serialized.Exists(x => x.PropertyName == name) && !creator.Contains(name)))
                this.Add(type, name, ContractMismatchKind.NotSerialized, "The schema describes it and Newtonsoft neither writes nor reads it.");

            if (isInput)
            {
                foreach (var (name, propertySchema) in properties)
                {
                    var json = contract.Properties.GetClosestMatchProperty(name);
                    var writable = creator.Contains(name) || (json is not null && !json.Ignored && json.Writable);

                    if (writable && propertySchema.ReadOnly)
                        this.Add(type, name, ContractMismatchKind.ReadOnlyButWritable, "Newtonsoft reads it from the body and the schema marks it read-only.");

                    if (!writable && !propertySchema.ReadOnly && serialized.Exists(x => x.PropertyName == name))
                        this.Add(type, name, ContractMismatchKind.WritableButIgnored, "The schema lets a client send it and Newtonsoft ignores it.");
                }
            }

            foreach (var property in contract.Properties.Where(x => !x.Ignored && x.PropertyType is not null))
                this.Visit(property.PropertyType, isInput);
        }

        private void VerifyConverted(Type type, OpenApiSchema schema)
        {
            if (schema is null)
                return;

            if (schema.Type is null or "object" || schema.Properties?.Count > 0)
            {
                this.Add(type, string.Empty, ContractMismatchKind.ConverterNotDescribedAsValue, $"A JSON converter writes it as a single value and the schema describes it as '{schema.Type ?? "anything"}'.");

                return;
            }

            if (schema.Example is not OpenApiString example)
                return;

            try
            {
                JsonConvert.DeserializeObject(JsonConvert.ToString(example.Value), type, settings);
            }
            catch (JsonException exception)
            {
                this.Add(type, string.Empty, ContractMismatchKind.InvalidExample, $"The example \"{example.Value}\" cannot be read: {exception.Message}");
            }
        }

        private bool HasConverter(Type type) => resolver.ResolveContract(type).Converter is not null || settings.Converters.Any(x => x.CanConvert(type));

        /// <summary>
        /// El esquema real detrás de una referencia o de un <c>allOf</c> con una sola referencia.
        /// </summary>
        private OpenApiSchema Resolve(OpenApiSchema schema)
        {
            if (schema?.Reference is not null && repository.Schemas.TryGetValue(schema.Reference.Id, out var referenced))
                return referenced;

            if (schema?.AllOf?.Count == 1)
                return this.Resolve(schema.AllOf[0]);

            return schema;
        }

        private void Add(Type type, string property, ContractMismatchKind kind, string detail) => this.Mismatches.Add(new ContractMismatch(type, property, kind, detail));

        /// <summary>
        /// Tipos que no son del micro: los simples, los del framework y los que describe el propio Swashbuckle.
        /// </summary>
        private static bool IsFramework(Type type) =>
            type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(Guid)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Uri)
            || type == typeof(object)
            || type == typeof(ProblemDetails)
            || (type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true && !type.IsGenericType)
            || type.Namespace?.StartsWith("Microsoft", StringComparison.Ordinal) == true
            || type.Namespace?.StartsWith("Newtonsoft", StringComparison.Ordinal) == true;
    }
}
