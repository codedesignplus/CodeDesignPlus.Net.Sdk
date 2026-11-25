using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Pone el nombre a cada valor de un enum sin cambiar su tipo.
/// </summary>
/// <remarks>
/// Los enums viajan como entero en los 37 micros y así deben seguir. El esquema solo traía <c>[0, 1, 2]</c>, y quien
/// lo lee no sabe qué significa cada número (pendings/753). Se añade <c>x-enum-varnames</c>, la extensión que
/// entienden los generadores de clientes, y la misma lista en la descripción para quien solo lee el texto.
/// </remarks>
public sealed class EnumNamesSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// El nombre de la extensión con los nombres de los valores, en el mismo orden que <c>enum</c>.
    /// </summary>
    public const string VarNamesExtension = "x-enum-varnames";

    /// <summary>
    /// Aplica el filtro al esquema de un enum.
    /// </summary>
    /// <param name="schema">El esquema generado.</param>
    /// <param name="context">El contexto con el tipo del esquema.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;

        if (!type.IsEnum || schema.Enum is null || schema.Enum.Count == 0)
            return;

        var members = Enum.GetValues(type)
            .Cast<object>()
            .Select(value => (Value: Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture), Name: Enum.GetName(type, value)))
            .ToList();

        // Mismo orden que schema.Enum: se recorre el esquema, no el tipo, por si el generador cambia el orden.
        var names = new OpenApiArray();

        foreach (var item in schema.Enum)
        {
            var value = item switch
            {
                OpenApiInteger integer => integer.Value,
                OpenApiLong @long => @long.Value,
                _ => (long?)null
            };

            var member = members.FirstOrDefault(x => x.Value == value);

            names.Add(new OpenApiString(member.Name ?? value?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty));
        }

        schema.Extensions[VarNamesExtension] = names;

        var legend = string.Join(", ", members.Select(x => $"{x.Value} = {x.Name}"));

        schema.Description = string.IsNullOrWhiteSpace(schema.Description) ? legend : $"{schema.Description}\n\n{legend}";
    }
}
