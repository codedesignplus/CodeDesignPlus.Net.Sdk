using CodeDesignPlus.Net.Exceptions.Models;

namespace CodeDesignPlus.Net.Exceptions.Test.Models;

public class ErrorResponseTest
{
    [Fact]
    public void AddError_Should_Add_ErrorDetail_To_Errors_List()
    {
        // Arrange
        var errorResponse = new ErrorResponse("12345", Layer.Application);

        // Act
        errorResponse.AddError("E001", "Error message", "Field");

        // Assert
        Assert.Single(errorResponse.Errors);
        Assert.Equal("E001", errorResponse.Errors[0].Code);
        Assert.Equal("Field", errorResponse.Errors[0].Field);
        Assert.Equal("Error message", errorResponse.Errors[0].Message);
    }
}
