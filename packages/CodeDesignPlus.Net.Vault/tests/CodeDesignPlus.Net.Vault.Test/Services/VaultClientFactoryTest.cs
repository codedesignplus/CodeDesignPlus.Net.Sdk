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
    public async Task CreateClient_AfterAnotherClientSentARequest_DoesNotThrow()
    {
        // Arrange
        // Reproduce la caida de produccion: al compartir un HttpClient entre clientes, Polymath
        // asignaba BaseAddress en el constructor del segundo y HttpClient lo prohibe una vez ha
        // enviado la primera peticion. Pasaba en cada micro: el proveedor de configuracion lee
        // los secretos al arrancar y AddVault construye el segundo cliente justo despues.
        //
        // El puerto no escucha a proposito: lo que marca el HttpClient como usado es HABER
        // ENVIADO, no que la peticion tenga exito.
        var options = new VaultOptions
        {
            Address = "http://127.0.0.1:59999",
            Token = "root",
            AppName = "app-name"
        };

        options.Kubernetes.Enable = false;

        var first = VaultClientFactory.Create(options);

        try
        {
            await first.V1.System.GetHealthStatusAsync();
        }
        catch (Exception)
        {
            // La conexion falla y da igual: el HttpClient ya quedo marcado como usado.
        }

        // Act
        var second = VaultClientFactory.Create(options);

        // Assert
        Assert.NotNull(second);
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
    public void CreateClient_KubernetesAuth_ReturnVaultClient()
    {
        // Arrange
        var options = new VaultOptions
        {
            Address = "http://localhost:8200",
            AppName = "app-name"
        };

        options.Kubernetes.Enable = true;
        options.Kubernetes.PathTokenKubernetes = AppDomain.CurrentDomain.BaseDirectory + "/token";

        // Act
        var client = VaultClientFactory.Create(options);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void CreateClient_ThrowVaultException_InvalidAuth()
    {
        // Arrange
        var options = new VaultOptions
        {
            Address = "http://localhost:8200",
            AppName = "app-name"
        };

        // Act
        var exception = Assert.Throws<VaultException>(() => VaultClientFactory.Create(options));

        // Assert
        Assert.Equal("The authentication type is not defined.", exception.Message);
    }
}
