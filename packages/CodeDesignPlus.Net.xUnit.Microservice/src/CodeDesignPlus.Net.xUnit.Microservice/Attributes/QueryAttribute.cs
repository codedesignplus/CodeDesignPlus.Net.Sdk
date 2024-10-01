namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate queries.
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for queries.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class QueryAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var queries = typeof(TAssemblyScan).Assembly
            .GetTypes()
            .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
            .Where(x => x.GetInterfaces().ToList().Exists(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var query in queries)
        {
            var constructor = query.GetConstructors().FirstOrDefault()!;
            var parameters = constructor.GetParameters();
            var values = parameters.GetParameterValues();
            var instance = constructor.Invoke(values.Values.ToArray());

            yield return new object[] { query, instance, values };
        }
    }
}