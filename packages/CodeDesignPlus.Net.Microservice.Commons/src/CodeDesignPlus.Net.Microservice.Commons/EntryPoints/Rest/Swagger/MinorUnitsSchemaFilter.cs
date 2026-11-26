using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Declara como dinero en unidad menor todo <c>long</c> del contrato, salvo los marcados con <see cref="NotMoneyAttribute"/>.
/// </summary>
/// <remarks>
/// <para>
/// La regla 04 hace viajar todo monto en unidad menor como <c>long</c>. Los reportes lo devuelven como números sueltos
/// (<c>totalDue: 706167868</c>) y el esquema no decía la unidad: un agente lo leería como pesos, cien veces más
/// (Kappali pendings/754). La convención (regla 74): toda propiedad entera de 64 bits es dinero en centavos; los
/// contadores son <c>int</c> y quedan fuera, y el <c>long</c> que no es dinero lleva <see cref="NotMoneyAttribute"/>.
/// </para>
/// <para>
/// El atributo se lee a través del contrato de Newtonsoft con la configuración de MVC del micro, que es el que pone el
/// nombre del JSON. <c>File.Size</c> de File.Storage se excluye aquí por nombre, porque ese paquete no depende de
/// Core.Abstractions, donde vive el atributo.
/// </para>
/// </remarks>
/// <param name="options">La configuración de Newtonsoft de MVC.</param>
public sealed class MinorUnitsSchemaFilter(IOptions<MvcNewtonsoftJsonOptions> options) : ISchemaFilter
{
    /// <summary>
    /// La extensión que marca la unidad del monto, para quien lee el esquema por programa.
    /// </summary>
    public const string Extension = "x-money-unit";

    /// <summary>
    /// La descripción que se añade a cada monto en unidad menor.
    /// </summary>
    public const string Description = "Amount in the minor unit of the currency (cents): 12500050 is 125,000.50 COP.";

    /// <summary>
    /// Propiedades <c>long</c> de tipos del SDK sin acceso a <see cref="NotMoneyAttribute"/>, por nombre completo del tipo.
    /// </summary>
    private static readonly Dictionary<string, string[]> SdkNotMoney = new(StringComparer.Ordinal)
    {
        ["CodeDesignPlus.Net.File.Storage.Abstractions.Models.File"] = ["Size"]
    };

    private readonly IContractResolver contractResolver = options.Value.SerializerSettings.ContractResolver ?? new DefaultContractResolver();

    /// <summary>
    /// Aplica la convención al esquema de un tipo.
    /// </summary>
    /// <param name="schema">El esquema generado.</param>
    /// <param name="context">El contexto con el tipo del esquema.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (schema.Properties is null || schema.Properties.Count == 0)
            return;

        var contract = this.contractResolver.ResolveContract(context.Type) as JsonObjectContract;
        var sdkNotMoney = SdkNotMoney.GetValueOrDefault(context.Type.FullName ?? string.Empty) ?? [];

        foreach (var (name, property) in schema.Properties)
        {
            if (property.Type != "integer" || property.Format != "int64")
                continue;

            var member = contract?.Properties.GetClosestMatchProperty(name);

            if (IsNotMoney(member) || sdkNotMoney.Contains(member?.UnderlyingName ?? name, StringComparer.OrdinalIgnoreCase))
                continue;

            Mark(property);
        }
    }

    private static bool IsNotMoney(JsonProperty member) =>
        member?.AttributeProvider?.GetAttributes(typeof(NotMoneyAttribute), true).Count > 0;

    private static void Mark(OpenApiSchema property)
    {
        property.Extensions[Extension] = new OpenApiString("minor");

        // MoneySchemaFilter ya describe Money.amount; no se repite.
        if (property.Description?.Contains("minor unit", StringComparison.OrdinalIgnoreCase) == true)
            return;

        property.Description = string.IsNullOrWhiteSpace(property.Description) ? Description : $"{property.Description} {Description}";
    }
}
