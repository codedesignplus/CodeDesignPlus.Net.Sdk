namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating entities.
/// </summary>
public class EntityTest
{
    /// <summary>
    /// Validates that entities can be created and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [Entity<Errors>]
    public void Entity_Properties_ShouldBeSetAndRetrievedCorrectly(Type entity, object instance)
    {
        // Assert
        Assert.NotNull(instance);

        Assert.All(entity.GetProperties(), property =>
        {
            var value = property.GetValue(instance);
            var valueDefault = property.PropertyType.GetDefaultValue();
            Assert.NotEqual(valueDefault, value);
        });
    }
}