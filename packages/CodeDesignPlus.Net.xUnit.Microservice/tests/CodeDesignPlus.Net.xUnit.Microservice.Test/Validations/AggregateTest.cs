namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating aggregates.
/// </summary>
public class AggregateTest
{
    /// <summary>
    /// Validates that aggregates can be created using the constructor and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [Aggregate<Errors>(false)]
    public void Aggregate_Constructor_ShouldSetAndRetrievePropertiesCorrectly(Type aggregate, object instance, Dictionary<ParameterInfo, object> values)
    {
        // Assert
        Assert.NotNull(instance);

        var value = aggregate.GetProperty(nameof(AggregateRoot.Id))!.GetValue(instance, null);
        var valueExpected = values.First(x => x.Key.Name!.Equals(nameof(AggregateRoot.Id), StringComparison.OrdinalIgnoreCase)).Value;
        Assert.Equal(valueExpected, value);
    }

    /// <summary>
    /// Validates that aggregates can be created using the named constructor with custom values.
    /// </summary>
    [Theory]
    [Aggregate<Errors>(true)]
    public void Aggregate_CreateMethod_ShouldCreateInstanceWithCustomValues(Type aggregate, object instance, Dictionary<ParameterInfo, object> values)
    {
        // Assert
        Assert.NotNull(instance);

        var value = aggregate.GetProperty(nameof(AggregateRoot.Id))!.GetValue(instance, null);
        var valueExpected = values.First(x => x.Key.Name!.Equals(nameof(AggregateRoot.Id), StringComparison.OrdinalIgnoreCase)).Value;
        Assert.Equal(valueExpected, value);
    }
}