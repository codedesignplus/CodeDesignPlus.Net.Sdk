namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate entities.
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for entities.</typeparam>
public class EntityAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var entities = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.FullName.StartsWith("Castle"))
            .Where(x => typeof(IEntityBase).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract && !x.IsSubclassOf(typeof(AggregateRoot)))
            .ToList();

        foreach (var entity in entities)
        {
            var instance = Activator.CreateInstance(entity);

            entity.SetValueProperties(instance);

            yield return new object[] { entity, instance };
        }
    }
}