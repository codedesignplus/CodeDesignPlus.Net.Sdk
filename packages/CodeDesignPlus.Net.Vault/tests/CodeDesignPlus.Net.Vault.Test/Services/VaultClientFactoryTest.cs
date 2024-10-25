using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.Vault.Services;

namespace CodeDesignPlus.Net.Vault.Test.Services;

public class VaultClientFactoryTest()
{
    [Fact]
    public void CreateClient_OptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        VaultOptions options = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => VaultClientFactory.Create(options));
        
        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'options')", exception.Message);
    }

    [Fact]
    public void CreateClient_KubernetesEnableIsFalse_ReturnVaultClient()
    {
        // Arrange
        var options = new VaultOptions
        {
            Address = "http://localhost:8200",
            RoleId = "role-id",
            SecretId = "secret-id",
            AppName = "app-name"
        };

        options.Kubernetes.Enable = false;

        // Act
        var client = VaultClientFactory.Create(options);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void CreateClient_KubernetesEnableIsTrue_ReturnVaultClient()
    {
        // Arrange
        var options = new VaultOptions
        {
            Address = "http://localhost:8200",
            RoleId = "role-id",
            SecretId = "secret-id",
            AppName = "app-name"
        };

        options.Kubernetes.Enable = true;
        options.Kubernetes.PathTokenKubernetes = AppDomain.CurrentDomain.BaseDirectory + "/token";

        // Act
        var client = VaultClientFactory.Create(options);

        // Assert
        Assert.NotNull(client);
    }
}
