using System.Collections;
using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.xUnit.Cache;

/// <summary>
/// Comprueba que un DTO que se guarda en el cache vuelve igual que entró.
/// </summary>
/// <remarks>
/// El cache (<c>CodeDesignPlus.Net.Redis.Cache</c>) serializa con <see cref="Serializers.JsonSerializer"/>, que es
/// Newtonsoft con su configuración por defecto: construye el objeto con el constructor marcado
/// <see cref="JsonConstructorAttribute"/>, o el público sin parámetros, o el único público con parámetros, y después solo
/// asigna las propiedades con <c>set</c> público (<c>init</c> cuenta) o marcadas con <see cref="JsonPropertyAttribute"/>.
/// Una propiedad con <c>set</c> privado que no es parámetro de ese constructor vuelve con su valor por defecto, sin un
/// error: así se perdió en un detalle cuánto de un pago fue saldo a favor, y así volvía vacío el detalle de un correo.
/// <para>
/// El inspector recorre por reflexión todo lo alcanzable desde el DTO (propiedades, elementos de listas y diccionarios)
/// y aplica esas mismas reglas. Las propiedades calculadas, sin campo detrás, no cuentan: se recalculan al leer.
/// </para>
/// </remarks>
public static class CacheContract
{
    private static readonly Type[] LeafTypes = [typeof(string), typeof(Guid), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Uri), typeof(object)];

    /// <summary>
    /// Lista cada propiedad alcanzable desde <paramref name="root"/> que no volvería del cache.
    /// </summary>
    /// <param name="root">El tipo del DTO.</param>
    /// <returns>«Tipo.Propiedad» por cada propiedad perdida, o «Tipo: motivo» si el tipo no se puede construir; vacía si no se pierde nada.</returns>
    public static IReadOnlyList<string> FindLostProperties(Type root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var lost = new List<string>();
        var visited = new HashSet<Type>();

        Visit(root, visited, lost);

        return lost;
    }

    /// <summary>
    /// Pasa el inspector por todos los DTO (<see cref="IDtoBase"/>) concretos de un ensamblado.
    /// </summary>
    /// <param name="assembly">El ensamblado con la capa de aplicación del micro.</param>
    /// <returns>«Dto → Tipo.Propiedad» por cada propiedad perdida; vacía si ninguno pierde nada.</returns>
    /// <remarks>
    /// Se pasan todos, se cacheen hoy o no: cualquiera puede empezar a cachearse mañana sin que nadie mire esto.
    /// </remarks>
    public static IReadOnlyList<string> FindLostPropertiesInDtos(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return assembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && !x.ContainsGenericParameters && typeof(IDtoBase).IsAssignableFrom(x))
            .OrderBy(x => x.FullName, StringComparer.Ordinal)
            .SelectMany(dto => FindLostProperties(dto).Select(property => $"{dto.Name} → {property}"))
            .ToList();
    }

    /// <summary>
    /// Hace la ida y vuelta real del cache: serializa y deserializa con el mismo serializador.
    /// </summary>
    /// <typeparam name="T">El tipo del DTO.</typeparam>
    /// <param name="value">El DTO tal como lo guarda la consulta.</param>
    /// <returns>El DTO tal como lo leería la siguiente consulta.</returns>
    public static T RoundTrip<T>(T value) => Serializers.JsonSerializer.Deserialize<T>(Serializers.JsonSerializer.Serialize(value));

    private static void Visit(Type type, HashSet<Type> visited, List<string> lost)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (IsLeaf(type) || !visited.Add(type))
        {
            return;
        }

        var elements = ElementTypes(type);

        if (elements is not null)
        {
            foreach (var element in elements)
            {
                Visit(element, visited, lost);
            }

            return;
        }

        if (!TryChooseConstructorParameters(type, out var parameters))
        {
            lost.Add($"{type.Name}: no constructor Newtonsoft can use");

            return;
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length > 0 || property.GetCustomAttribute<JsonIgnoreAttribute>() is not null || IsComputed(type, property))
            {
                continue;
            }

            var restorable = property.SetMethod?.IsPublic == true || parameters.Contains(property.Name) || property.GetCustomAttribute<JsonPropertyAttribute>() is not null;

            if (!restorable)
            {
                lost.Add($"{type.Name}.{property.Name}");
            }

            Visit(property.PropertyType, visited, lost);
        }
    }

    /// <summary>
    /// Una propiedad de solo lectura sin campo generado por el compilador se calcula a partir de las demás: no hay nada
    /// que restaurar.
    /// </summary>
    private static bool IsComputed(Type type, PropertyInfo property) => property.SetMethod is null && FindBackingField(type, property.Name) is null;

    private static FieldInfo FindBackingField(Type type, string propertyName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var field = current.GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (field is not null)
            {
                return field;
            }
        }

        return null;
    }

    /// <summary>
    /// Elige el constructor como lo elige Newtonsoft y devuelve los nombres de sus parámetros.
    /// </summary>
    private static bool TryChooseConstructorParameters(Type type, out HashSet<string> parameters)
    {
        parameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (type.IsAbstract || type.IsInterface)
        {
            return false;
        }

        var all = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var marked = all.FirstOrDefault(x => x.GetCustomAttribute<JsonConstructorAttribute>() is not null);
        var publicOnes = all.Where(x => x.IsPublic).ToList();

        var chosen = marked
            ?? publicOnes.FirstOrDefault(x => x.GetParameters().Length == 0)
            ?? (publicOnes.Count == 1 ? publicOnes[0] : null);

        if (chosen is not null)
        {
            parameters.UnionWith(chosen.GetParameters().Select(x => x.Name!));

            return true;
        }

        // Un struct siempre se puede crear con sus valores por defecto, aunque no declare un constructor sin parámetros.
        return type.IsValueType;
    }

    private static Type[] ElementTypes(Type type)
    {
        if (type.IsArray)
        {
            return [type.GetElementType()!];
        }

        if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
        {
            return type.GetGenericArguments();
        }

        return null;
    }

    private static bool IsLeaf(Type type) => type.IsPrimitive || type.IsEnum || LeafTypes.Contains(type) || type.Namespace?.StartsWith("NodaTime", StringComparison.Ordinal) == true;
}
