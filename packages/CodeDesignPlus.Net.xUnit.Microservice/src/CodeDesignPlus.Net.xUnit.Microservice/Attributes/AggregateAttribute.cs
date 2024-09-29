namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate aggregates.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AggregateAttribute"/> class.
/// </remarks>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for aggregates.</typeparam>
/// <param name="useCreateMethod">Indicates whether to use the static Create method or the constructor to create instances of aggregates.</param>
public class AggregateAttribute<TAssemblyScan>(bool useCreateMethod) : DataAttribute where TAssemblyScan: IErrorCodes
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var aggregates = typeof(TAssemblyScan).Assembly
            .GetTypes()
            .Where(x => !x.FullName.StartsWith("Castle"))
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(AggregateRoot)))
            .ToList();

        foreach (var aggregate in aggregates)
        {
            object instance;
            Dictionary<ParameterInfo, object> values;

            if (useCreateMethod)
            {
                var nameConstructor = aggregate.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);

                if (nameConstructor is null)
                    throw new CoreException($"The {aggregate.Name} class does not have a static Create method.");

                values = nameConstructor.GetParameters().GetParameterValues();
                instance = nameConstructor.Invoke(null, [.. values.Values]);
            }
            else
            {
                var constructor = aggregate.GetConstructor([typeof(Guid)]);
                values = new Dictionary<ParameterInfo, object>
                {
                    { constructor.GetParameters().First(), Guid.NewGuid() }
                };
                instance = constructor.Invoke([.. values.Values]);
            }

            yield return new object[] { aggregate, instance, values };
        }
    }
}