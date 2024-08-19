namespace CodeDesignPlus.Net.EFCore.Test.Options;

public class EFCoreOptionTest
{
    private readonly EFCoreOptions efCoreOption = new()
    {
        ClaimsIdentity = new ClaimsOption()
        {
            Email = nameof(ClaimsOption.Email),
            IdUser = nameof(ClaimsOption.IdUser),
            Role = nameof(ClaimsOption.Role),
            User = nameof(ClaimsOption.User),
        }
    };

    [Fact]
    public void Properties_AccessorsAndDataAnnotations_IsValid()
    {
        // Act
        var results = this.efCoreOption.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(nameof(ClaimsOption.Email), this.efCoreOption.ClaimsIdentity.Email);
        Assert.Equal(nameof(ClaimsOption.IdUser), this.efCoreOption.ClaimsIdentity.IdUser);
        Assert.Equal(nameof(ClaimsOption.Role), this.efCoreOption.ClaimsIdentity.Role);
        Assert.Equal(nameof(ClaimsOption.User), this.efCoreOption.ClaimsIdentity.User);
    }

    [Theory]
    [InlineData(nameof(EFCoreOptions.ClaimsIdentity))]
    public void Properties_SetNullProperty_PropertyNullIsNotValid(string property)
    {
        // Arrange
        this.efCoreOption.GetType()?.GetProperty(property)?.SetValue(this.efCoreOption, null);

        // Act
        var results = this.efCoreOption.Validate();

        // Assert
        Assert.NotEmpty(results);
        //The Email field is required.
        Assert.Equal($"The {property} field is required.", results.Select(x => x.ErrorMessage).FirstOrDefault());
    }
}