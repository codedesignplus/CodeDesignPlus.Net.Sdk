namespace CodeDesignPlus.Net.EFCore.Test.Options;

public class ClaimsOptionTest
{
    private readonly ClaimsOption claimsOption = new()
    {
        Email = nameof(ClaimsOption.Email),
        IdUser = nameof(ClaimsOption.IdUser),
        Role = nameof(ClaimsOption.Role),
        User = nameof(ClaimsOption.User),
    };

    [Fact]
    public void Properties_AccessorsAndDataAnnotations_IsValid()
    {
        // Act
        var results = this.claimsOption.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(nameof(ClaimsOption.Email), this.claimsOption.Email);
        Assert.Equal(nameof(ClaimsOption.IdUser), this.claimsOption.IdUser);
        Assert.Equal(nameof(ClaimsOption.Role), this.claimsOption.Role);
        Assert.Equal(nameof(ClaimsOption.User), this.claimsOption.User);
    }

    [Theory]
    [InlineData(nameof(ClaimsOption.Email))]
    [InlineData(nameof(ClaimsOption.IdUser))]
    [InlineData(nameof(ClaimsOption.Role))]
    [InlineData(nameof(ClaimsOption.User))]
    public void Properties_SetNullProperty_PropertyNullIsNotValid(string property)
    {
        // Arrange
        this.claimsOption.GetType()?.GetProperty(property)?.SetValue(this.claimsOption, null);

        // Act
        var results = this.claimsOption.Validate();

        // Assert
        Assert.NotEmpty(results);
        //The Email field is required.
        Assert.Equal($"The {property} field is required.", results.Select(x => x.ErrorMessage).FirstOrDefault());
    }
}