namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating data transfer objects (DTOs).
/// </summary>
public class DataTransferObjectTests
{
    /// <summary>
    /// Validates that DTOs can be created and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [DataTransferObject]
    public void Dtos_GetAndSet_Success(Type dto, object instance)
    {
        // Assert
        Assert.NotNull(instance);

        var properties = dto.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(instance);
            Assert.NotNull(value);
            Assert.NotEqual(value, property.PropertyType.GetDefaultValue());
        }
    }
}