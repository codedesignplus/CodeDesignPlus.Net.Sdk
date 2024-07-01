using CodeDesignPlus.Net.Exceptions.Models;

namespace CodeDesignPlus.Net.Exceptions.Test.Models;

public class ErrorDetailTest
{
    [Fact]
    public void ErrorDetail_CanBeCreated()
    {
        // Arrange
        string code = "E-001";
        string field = "FieldName";
        string message = "Error message";

        // Act
        var errorDetail = new ErrorDetail(code, field, message);

        // Assert
        Assert.Equal(code, errorDetail.Code);
        Assert.Equal(field, errorDetail.Field);
        Assert.Equal(message, errorDetail.Message);
    }
}
