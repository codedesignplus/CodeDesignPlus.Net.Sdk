namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating queries.
/// </summary>
public class QueriesTests
{
    /// <summary>
    /// Validates that queries can be created and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [Query]
    public void Queries_GetAndSet_Success(Type query, object instance, Dictionary<ParameterInfo, object> values)
    {
        // Assert
        Assert.NotNull(instance);

        var properties = query.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(instance);
            var valueExpected = values.FirstOrDefault(x => x.Key.Name == property.Name).Value;

            Assert.NotNull(value);
            Assert.Equal(valueExpected, value);
        }
    }
}