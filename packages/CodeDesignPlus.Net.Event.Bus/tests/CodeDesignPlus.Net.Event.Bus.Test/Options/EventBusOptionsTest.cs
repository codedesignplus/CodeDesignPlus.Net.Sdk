namespace CodeDesignPlus.Net.Event.Bus.Test.Options;

public class EventBusOptionsTest
{
    [Fact]
    public void EventBusOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventBusOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void EventBusOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new EventBusOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void EventBusOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new EventBusOptions()
        {
            Enable = true,
            Name = Guid.NewGuid().ToString(),
            Email = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    }

    [Fact]
    public void EventBusOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new EventBusOptions()
        {
            Enable = true,
            Name = Guid.NewGuid().ToString(),
            Email = "asdfasdfsdfgs"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    }
}
