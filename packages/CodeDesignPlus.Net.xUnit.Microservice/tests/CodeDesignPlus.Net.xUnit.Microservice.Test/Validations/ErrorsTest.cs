namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating error formats.
/// </summary>
public class ErrorTests
{
    /// <summary>
    /// Validates that error messages follow the correct format.
    /// </summary>
    [Theory]
    [Errors]
    public void Errors_CheckFormat_Success(FieldInfo error, object value)
    {
        // Assert
        Assert.NotNull(error);
        Assert.NotNull(value);
        Assert.NotEmpty(value.ToString()!);

        var pattern = @"^\d{3} : .+$";
        Assert.Matches(pattern, value.ToString());
    }
}