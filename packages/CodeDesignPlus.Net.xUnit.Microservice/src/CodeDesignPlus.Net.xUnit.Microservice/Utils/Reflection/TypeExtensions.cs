namespace CodeDesignPlus.Net.xUnit.Microservice.Utils.Reflection;

/// <summary>
/// Provides extension methods for working with types and reflection.
/// </summary>
public static class TypeExtensions
{
    private static readonly Dictionary<Type, Func<object>> defaultValues = new()
    {
        { typeof(string), () => "Test" },
        { typeof(int), () => 1 },
        { typeof(long), () => 1L },
        { typeof(Guid), () => Guid.NewGuid() },
        { typeof(DateTime), () => DateTime.UtcNow },
        { typeof(DateTimeOffset), () => DateTimeOffset.UtcNow },
        { typeof(bool), () => true },
        { typeof(decimal), () => 1.0M },
        { typeof(float), () => 1.0F },
        { typeof(double), () => 1.0D },
        { typeof(byte), () => (byte)1 },
        { typeof(short), () => (short)1 },
        { typeof(byte[]), () => new byte[] { 1, 2, 3 } },
        { typeof(Dictionary<string, object>), () => new Dictionary<string, object>() },
        { typeof(char), () => 'A' },
        { typeof(uint), () => 1U },
        { typeof(ulong), () => 1UL },
        { typeof(ushort), () => (ushort)1 },
        { typeof(sbyte), () => (sbyte)1 },
        { typeof(TimeSpan), () => TimeSpan.Zero },
        { typeof(Uri), () => new Uri("https://codedesignplus.com") }
    };

    /// <summary>
    /// Gets the default value for the specified type.
    /// </summary>
    /// <param name="type">The type to get the default value for.</param>
    /// <returns>The default value for the specified type.</returns>
    public static object GetDefaultValue(this Type type)
    {
        return type!.IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// Gets the parameter values for the specified array of parameter information.
    /// </summary>
    /// <param name="properties">The array of parameter information.</param>
    /// <returns>A dictionary of parameter information and their corresponding values.</returns>
    public static Dictionary<ParameterInfo, object> GetParameterValues(this ParameterInfo[] properties)
    {
        var values = new Dictionary<ParameterInfo, object>();

        foreach (var property in properties)
        {
            if (defaultValues.TryGetValue(property.ParameterType, out var valueFactory))
            {
                values.Add(property, valueFactory());
            }
            else if (property.ParameterType.IsEnum)
            {
                values.Add(property, Enum.GetValues(property.ParameterType).GetValue(0)!);
            }
            else if (property.ParameterType.IsClass && !property.ParameterType.IsAbstract)
            {
                values.Add(property, Activator.CreateInstance(property.ParameterType)!);
            }
            else if (property.ParameterType.IsGenericType && property.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(property.ParameterType);

                if (underlyingType != null && defaultValues.TryGetValue(underlyingType, out valueFactory))
                {
                    values.Add(property, valueFactory());
                }
            }
        }

        return values;
    }

    /// <summary>
    /// Sets the property values for the specified instance of the specified type.
    /// </summary>
    /// <param name="type">The type of the instance.</param>
    /// <param name="instance">The instance to set the property values for.</param>
    public static void SetValueProperties(this Type type, object instance)
    {
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (defaultValues.TryGetValue(property.PropertyType, out var valueFactory))
            {
                property.SetValue(instance, valueFactory());
            }
            else if (property.PropertyType.IsEnum)
            {
                property.SetValue(instance, Enum.GetValues(property.PropertyType).GetValue(0)!);
            }
            else if (property.PropertyType.IsClass && !property.PropertyType.IsAbstract)
            {
                property.SetValue(instance, Activator.CreateInstance(property.PropertyType)!);
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                if (underlyingType != null && defaultValues.TryGetValue(underlyingType, out valueFactory))
                {
                    property.SetValue(instance, valueFactory());
                }
            }
        }
    }
}