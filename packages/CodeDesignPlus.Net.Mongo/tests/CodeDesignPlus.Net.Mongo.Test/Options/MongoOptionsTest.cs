using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Mongo.Test.Options;

public class MongoOptionsTest
{
    [Fact]
    public void MongoOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new MongoOptions()
        {
            ConnectionString = Guid.NewGuid().ToString(),
            Database = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MongoOptions_ConnectionStringIsRequired_FailedValidation()
    {
        // Arrange
        var options = new MongoOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The ConnectionString field is required.");
    }

    [Fact]
    public void MongoOptions_DatabaseIsRequired_FailedValidation()
    {
        // Arrange
        var options = new MongoOptions()
        {
            Enable = true,
            ConnectionString = Guid.NewGuid().ToString(),
            Database = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Database field is required.");
    }
}
