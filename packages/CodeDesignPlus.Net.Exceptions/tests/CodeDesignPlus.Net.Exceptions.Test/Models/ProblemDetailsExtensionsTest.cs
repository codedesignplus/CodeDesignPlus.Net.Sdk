using CodeDesignPlus.Net.Exceptions.Models;

namespace CodeDesignPlus.Net.Exceptions.Test.Models;

public class ProblemDetailsExtensionsTest
{
    [Fact]
    public void InvalidParamDetail_CanBeCreated_WithRequiredParameters()
    {
        // Arrange
        var name = "username";
        var reason = "Must not be empty";

        // Act
        var detail = new InvalidParamDetail(name, reason);

        // Assert
        Assert.Equal(name, detail.Name);
        Assert.Equal(reason, detail.Reason);
        Assert.Null(detail.Code);
    }

    [Fact]
    public void InvalidParamDetail_CanBeCreated_WithAllParameters()
    {
        // Arrange
        var name = "email";
        var reason = "Invalid format";
        var code = "ERR001";

        // Act
        var detail = new InvalidParamDetail(name, reason, code);

        // Assert
        Assert.Equal(name, detail.Name);
        Assert.Equal(reason, detail.Reason);
        Assert.Equal(code, detail.Code);
    }

    [Fact]
    public void InvalidParamDetail_Equality_WorksAsExpected()
    {
        // Arrange
        var detail1 = new InvalidParamDetail("field", "reason", "code");
        var detail2 = new InvalidParamDetail("field", "reason", "code");
        var detail3 = new InvalidParamDetail("field", "other reason", "code");

        // Act & Assert
        Assert.Equal(detail1, detail2);
        Assert.NotEqual(detail1, detail3);
    }

    [Fact]
    public void InvalidParamDetail_WithNullCode_Equality_WorksAsExpected()
    {
        // Arrange
        var detail1 = new InvalidParamDetail("field", "reason");
        var detail2 = new InvalidParamDetail("field", "reason", null);

        // Act & Assert
        Assert.Equal(detail1, detail2);
    }
}
