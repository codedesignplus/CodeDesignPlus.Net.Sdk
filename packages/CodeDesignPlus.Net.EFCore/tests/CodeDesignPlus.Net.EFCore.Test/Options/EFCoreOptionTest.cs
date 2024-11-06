namespace CodeDesignPlus.Net.EFCore.Test.Options;

public class EFCoreOptionTest
{
    private readonly EFCoreOptions efCoreOption = new()
    {
        Enable = true,
        RegisterRepositories = true
    };

    [Fact]
    public void Properties_AccessorsAndDataAnnotations_IsValid()
    {
        // Act
        var results = this.efCoreOption.Validate();

        // Assert
        Assert.Empty(results);
    }
}