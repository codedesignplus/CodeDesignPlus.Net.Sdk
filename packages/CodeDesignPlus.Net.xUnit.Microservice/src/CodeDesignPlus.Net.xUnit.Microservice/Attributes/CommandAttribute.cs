namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate commands.
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for commands.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var commands = typeof(TAssemblyScan).Assembly
            .GetTypes()
            .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
            .Where(x => typeof(IRequest).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var command in commands)
        {
            var constructor = command.GetConstructors().FirstOrDefault()!;
            var parameters = constructor.GetParameters();
            var values = parameters.GetParameterValues();
            var instance = Activator.CreateInstance(command, values.Values.ToArray());

            yield return new object[] { command, instance, values };
        }
    }
}