using CodeDesignPlus.Net.Vault.Abstractions.Options;

namespace CodeDesignPlus.Net.Vault.Test.Options;

public class VaultOptionsTest
{
    // [Fact]
    // public void VaultOptions_DefaultValues_Valid()
    // {
    //     // Arrange
    //     var options = new VaultOptions()
    //     {
    //         Name = Guid.NewGuid().ToString()
    //     };

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Empty(results);
    // }

    // [Fact]
    // public void VaultOptions_NameIsRequired_FailedValidation()
    // {
    //     // Arrange
    //     var options = new VaultOptions();

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    // }

    // [Fact]
    // public void VaultOptions_EmailIsRequired_FailedValidation()
    // {
    //     // Arrange
    //     var options = new VaultOptions()
    //     {
    //         Enable = true,
    //         Name = Guid.NewGuid().ToString(),
    //         Email = null
    //     };

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    // }

    // [Fact]
    // public void VaultOptions_EmailIsInvalid_FailedValidation()
    // {
    //     // Arrange
    //     var options = new VaultOptions()
    //     {
    //         Enable = true,
    //         Name = Guid.NewGuid().ToString(),
    //         Email = "asdfasdfsdfgs"
    //     };

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    // }
}
