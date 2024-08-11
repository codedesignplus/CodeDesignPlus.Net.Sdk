namespace CodeDesignPlus.Net.Mongo.Diagnostics.Test.Options;

public class MongoDiagnosticsOptionsTest
{
    [Fact]
    public void MongoDiagnosticsOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new MongoDiagnosticsOptions()
        {

        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
