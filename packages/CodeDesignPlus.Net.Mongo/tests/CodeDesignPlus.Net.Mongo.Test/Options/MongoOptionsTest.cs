using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Mongo.Test.Options;

public class MongoOptionsTest
{
    [Fact]
    public void MongoOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new MongoOptions()
        {
            ConnectionString = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MongoOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new MongoOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void MongoOptions_EmailIsRequired_FailedValidation()
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
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    }

    [Fact]
    public void MongoOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new MongoOptions()
        {
            Enable = true,
            ConnectionString = Guid.NewGuid().ToString(),
            Database = "asdfasdfsdfgs"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    }
}
