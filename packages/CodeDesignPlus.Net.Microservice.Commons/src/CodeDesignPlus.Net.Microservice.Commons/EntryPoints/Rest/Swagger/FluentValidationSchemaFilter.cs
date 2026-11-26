using System.Collections.Concurrent;
using System.Globalization;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Lleva al esquema las reglas de FluentValidation: obligatorios, longitudes, rangos y patrones.
/// </summary>
/// <remarks>
/// <para>
/// Las reglas viven en los validadores y el Swagger no las mostraba: ningún DTO de entrada de los 37 micros tenía
/// <c>required</c> ni <c>maxLength</c> (pendings/753). Quien llamaba se enteraba con el 400.
/// </para>
/// <para>
/// Los validadores son de los comandos, no de los DTO que llegan en el body: de 683 validadores, 636 son de un
/// comando y 3 de un DTO. Por eso el validador de un esquema se busca así: el del propio tipo si lo tiene (los
/// controllers que reciben el comando como body), y si no el del comando que se llama igual que el body sin su
/// sufijo (<c>CreateLevelDto</c> → <c>CreateLevelCommand</c>, <c>IssueDocumentRequest</c> → <c>IssueDocumentCommand</c>),
/// que es como los nombra el <c>MapsterConfig</c> de cada micro.
/// Las propiedades se emparejan por nombre.
/// </para>
/// <para>
/// Solo se trasladan las reglas incondicionales: una regla con <c>When</c> o <c>Unless</c> no hace obligatorio el
/// campo, porque depende de otros valores. Tampoco se siguen los validadores hijos (<c>SetValidator</c>,
/// <c>RuleForEach</c>): el esquema del tipo hijo recibe los suyos si tiene validador propio.
/// </para>
/// </remarks>
/// <param name="services">Los servicios de la aplicación, donde están registrados los validadores.</param>
public sealed class FluentValidationSchemaFilter(IServiceProvider services) : ISchemaFilter
{
    /// <summary>
    /// Los sufijos con que los micros nombran el body de un comando: <c>CreateLevelDto</c>, <c>IssueDocumentRequest</c>,
    /// <c>ChangeMeetingModalityBody</c>, <c>RateConfigurationInput</c>.
    /// </summary>
    private static readonly string[] BodySuffixes = ["Dto", "Request", "Body", "Input"];

    private static readonly ConcurrentDictionary<string, Type[]> TypesByName = new(StringComparer.Ordinal);

    private readonly ConcurrentDictionary<Type, IValidatorDescriptor> descriptors = new();

    /// <summary>
    /// Aplica las reglas del validador al esquema de un tipo.
    /// </summary>
    /// <param name="schema">El esquema generado.</param>
    /// <param name="context">El contexto con el tipo del esquema.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (schema.Properties is null || schema.Properties.Count == 0)
            return;

        var descriptor = this.descriptors.GetOrAdd(context.Type, this.FindDescriptor);

        if (descriptor is null)
            return;

        foreach (var member in descriptor.GetMembersWithValidators())
        {
            var (name, property) = schema.Properties.FirstOrDefault(x => string.Equals(x.Key, member.Key, StringComparison.OrdinalIgnoreCase));

            if (property is null)
                continue;

            foreach (var (validator, options) in member)
            {
                if (options.HasCondition || options.HasAsyncCondition)
                    continue;

                Apply(schema, name, property, validator);
            }
        }
    }

    private static void Apply(OpenApiSchema schema, string name, OpenApiSchema property, IPropertyValidator validator)
    {
        switch (validator)
        {
            case INotNullValidator:
                schema.Required.Add(name);
                property.Nullable = false;
                break;

            case INotEmptyValidator:
                schema.Required.Add(name);
                property.Nullable = false;

                if (property.Type == "string")
                    property.MinLength = Math.Max(property.MinLength ?? 0, 1);
                else if (property.Type == "array")
                    property.MinItems = Math.Max(property.MinItems ?? 0, 1);
                break;

            case ILengthValidator length when property.Type == "string":
                if (length.Min > 0)
                    property.MinLength = length.Min;

                if (length.Max > 0)
                    property.MaxLength = length.Max;
                break;

            case IRegularExpressionValidator regex when property.Type == "string":
                property.Pattern = regex.Expression;
                break;

            case IEmailValidator when property.Type == "string":
                property.Format = "email";
                break;

            case IBetweenValidator between when IsNumber(property):
                var exclusive = validator.Name.StartsWith("Exclusive", StringComparison.Ordinal);

                if (TryToDecimal(between.From, out var from))
                {
                    property.Minimum = from;
                    property.ExclusiveMinimum = exclusive;
                }

                if (TryToDecimal(between.To, out var to))
                {
                    property.Maximum = to;
                    property.ExclusiveMaximum = exclusive;
                }
                break;

            case IComparisonValidator comparison when IsNumber(property) && comparison.MemberToCompare is null && TryToDecimal(comparison.ValueToCompare, out var value):
                switch (comparison.Comparison)
                {
                    case Comparison.GreaterThan:
                        property.Minimum = value;
                        property.ExclusiveMinimum = true;
                        break;
                    case Comparison.GreaterThanOrEqual:
                        property.Minimum = value;
                        property.ExclusiveMinimum = false;
                        break;
                    case Comparison.LessThan:
                        property.Maximum = value;
                        property.ExclusiveMaximum = true;
                        break;
                    case Comparison.LessThanOrEqual:
                        property.Maximum = value;
                        property.ExclusiveMaximum = false;
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// El validador del propio tipo, o el del comando que se llama como el DTO.
    /// </summary>
    private IValidatorDescriptor FindDescriptor(Type type)
    {
        using var scope = services.CreateScope();

        var own = Resolve(scope.ServiceProvider, type);

        if (own is not null)
            return own;

        var suffix = BodySuffixes.FirstOrDefault(x => type.Name.Length > x.Length && type.Name.EndsWith(x, StringComparison.Ordinal));

        if (suffix is null)
            return null;

        var commandName = string.Concat(type.Name.AsSpan(0, type.Name.Length - suffix.Length), "Command");

        return FindTypes(commandName)
            .Select(command => Resolve(scope.ServiceProvider, command))
            .FirstOrDefault(x => x is not null);
    }

    private static IValidatorDescriptor Resolve(IServiceProvider provider, Type type)
    {
        if (type.IsGenericTypeDefinition || type.ContainsGenericParameters)
            return null;

        var validator = provider.GetService(typeof(IValidator<>).MakeGenericType(type)) as IValidator;

        return validator?.CreateDescriptor();
    }

    private static Type[] FindTypes(string name) => TypesByName.GetOrAdd(name, key => AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(x => !x.IsDynamic)
        .SelectMany(GetTypes)
        .Where(x => x.Name == key && !x.IsAbstract)
        .ToArray());

    private static IEnumerable<Type> GetTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(x => x is not null);
        }
    }

    private static bool IsNumber(OpenApiSchema property) => property.Type is "integer" or "number";

    private static bool TryToDecimal(object value, out decimal result)
    {
        result = 0;

        if (value is null || value is string || value is bool || value is char)
            return false;

        try
        {
            result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);

            return true;
        }
        catch (Exception exception) when (exception is InvalidCastException or FormatException or OverflowException)
        {
            return false;
        }
    }
}
