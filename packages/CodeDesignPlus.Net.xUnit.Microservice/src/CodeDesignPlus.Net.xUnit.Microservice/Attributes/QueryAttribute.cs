namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate queries.
/// </summary>
public class QueryAttribute : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var queries = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.FullName.StartsWith("Castle"))
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) && !x.IsInterface && !x.IsAbstract)
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