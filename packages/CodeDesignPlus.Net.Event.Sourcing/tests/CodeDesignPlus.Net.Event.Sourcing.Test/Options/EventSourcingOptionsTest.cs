namespace CodeDesignPlus.Net.Event.Sourcing.Test.Options;

public class EventSourcingOptionsTest
{
    [Fact]
    public void EventSourcingOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventSourcingOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void EventSourcingOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new EventSourcingOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void EventSourcingOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new EventSourcingOptions()
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
    public void EventSourcingOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new EventSourcingOptions()
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
