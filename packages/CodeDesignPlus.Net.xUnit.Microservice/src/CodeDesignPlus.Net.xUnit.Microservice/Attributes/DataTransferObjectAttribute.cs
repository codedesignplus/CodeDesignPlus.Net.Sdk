namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate data transfer objects (DTOs).
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for DTOs.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class DataTransferObjectAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var dtos = typeof(TAssemblyScan).Assembly
            .GetTypes()
            .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
            .Where(x => typeof(IDtoBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
            .ToList();

        foreach (var dto in dtos)
        {
            var instance = Activator.CreateInstance(dto);
            dto.SetValueProperties(instance);

            yield return new object[] { dto, instance };
        }
    }
}
