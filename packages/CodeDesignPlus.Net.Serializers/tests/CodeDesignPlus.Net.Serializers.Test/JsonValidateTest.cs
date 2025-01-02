namespace CodeDesignPlus.Net.Serializers.Test;

public class JsonValidateTest
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("{\"name\":\"John\"}", true)]
    [InlineData("{name:\"John\"}", false)]
    [InlineData("[{\"name\":\"John\"}]", true)]
    [InlineData("[{name:\"John\"}]", false)]
    public void IsValidJson_ShouldReturnExpectedResult(string? jsonString, bool expected)
    {
        // Act
        var result = JsonValidate.IsValidJson(jsonString);

        // Assert
        Assert.Equal(expected, result);
    }
}
