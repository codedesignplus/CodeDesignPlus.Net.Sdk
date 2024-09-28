namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating commands.
/// </summary>
public class CommandsTests
{
    /// <summary>
    /// Validates that commands can be created and their properties can be set and retrieved correctly.
    /// </summary>
    [Theory]
    [Command]
    public void Commands_GetAndSet_Success(Type command, object instance, Dictionary<ParameterInfo, object> values)
    {
        // Assert
        Assert.NotNull(instance);

        var properties = command.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(instance);
            var valueExpected = values.FirstOrDefault(x => x.Key.Name == property.Name).Value;

            Assert.NotNull(value);
            Assert.Equal(valueExpected, value);
        }
    }
}