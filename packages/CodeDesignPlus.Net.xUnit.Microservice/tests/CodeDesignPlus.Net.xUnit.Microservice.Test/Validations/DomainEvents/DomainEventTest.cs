using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.xUnit.Microservice.Utils.Reflection;
using CodeDesignPlus.Net.xUnit.Microservice.Validations.DomainEvents;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations.DomainEvents;

/// <summary>
/// A class for validating domain events.
/// </summary>
public class DomainEventTest
{
    /// <summary>
    /// Validates that domain events can be created using the constructor and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [DomainEvent(false)]
    public void DomainEvent_Constructor_ShouldSetAndRetrievePropertiesCorrectly(Type domainEvent, object instance, Dictionary<ParameterInfo, object> data)
    {
        // Assert
        Assert.NotNull(instance);

        Assert.All(domainEvent.GetProperties(), property =>
        {
            var value = property.GetValue(instance);
            var valueExpected = data.FirstOrDefault(x => x.Key.Name!.Equals(property.Name, StringComparison.OrdinalIgnoreCase)).Value;
            Assert.Equal(valueExpected, value);
        });
    }

    /// <summary>
    /// Validates that domain events can be created using the named constructor with custom values.
    /// </summary>
    [Theory]
    [DomainEvent(true)]
    public void DomainEvent_CreateMethod_ShouldCreateInstanceWithCustomValues(Type domainEvent, object instance, Dictionary<ParameterInfo, object> values)
    {
        // Assert
        Assert.NotNull(instance);

        var property = domainEvent.GetProperty(nameof(DomainEvent.AggregateId));
        var value = property!.GetValue(instance, null);
        var valueExpected = property.PropertyType.GetDefaultValue();
        Assert.NotEqual(valueExpected, value);
    }
}