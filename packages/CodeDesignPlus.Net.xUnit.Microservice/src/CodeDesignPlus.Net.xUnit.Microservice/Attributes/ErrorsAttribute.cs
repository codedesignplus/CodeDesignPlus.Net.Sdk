namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate error formats.
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for errors.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class ErrorsAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var errorClasses = typeof(TAssemblyScan).Assembly
            .GetTypes()
           .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
           .Where(x => typeof(IErrorCodes).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
           .ToList();

        foreach (var errorClass in errorClasses)
        {
            var errors = errorClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => x.FieldType == typeof(string))
                .ToList();

            foreach (var error in errors)
            {
                var value = error.GetValue(null);

                yield return new object[] { error, value };
            }
        }
    }
}