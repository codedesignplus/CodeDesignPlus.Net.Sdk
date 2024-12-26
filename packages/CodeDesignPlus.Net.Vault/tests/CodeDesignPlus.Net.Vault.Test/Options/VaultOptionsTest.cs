using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Vault.Test.Options;

public class VaultOptionsTest
{
    [Fact]
    public void VaultOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new VaultOptions()
        {
            Address = "http://localhost:8200",
            RoleId = "role-id",
            AppName = "app-name",
            SecretId = "secre",
            Solution = "solution-name"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void VaultOptions_InvalidValues_Failed()
    {
        // Arrange
        var options = new VaultOptions()
        {
            Address = null,
            RoleId = null,
            AppName = null,
            SecretId = null,
            Solution = null
        };

        options.Mongo.RoleSufix = null;
        options.Mongo.SufixMoundPoint = null;
        options.Mongo.TemplateConnectionString = null;

        options.Kubernetes.Enable = false;
        options.Kubernetes.RoleSufix = null;
        options.Kubernetes.SufixMoundPoint = null;

        options.RabbitMQ.RoleSufix = null;
        options.RabbitMQ.SufixMoundPoint = null;

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The Address field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The AppName field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Solution field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The RoleSufix field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The SufixMoundPoint field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The TemplateConnectionString field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The TypeAuth is required.");
    }

    [Fact]
    public void VaultOptions_ObjectsAreNull_Failed()
    {
        // Arrange
        var options = new VaultOptions()
        {
            Address = null,
            RoleId = null,
            AppName = null,
            SecretId = null,
            Solution = null,
            Mongo = null,
            Kubernetes = null,
            RabbitMQ = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The Address field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The AppName field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Solution field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Mongo field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Kubernetes field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The RabbitMQ field is required.");
    }

     [Fact]
    public void VaultOptions_KubernetesValues_Valid()
    {
        // Arrange
        var options = new VaultOptions()
        {
            Address = "http://localhost:8200",
            RoleId = null,
            AppName = "app-name",
            SecretId = null,
            Solution = "solution-name"
        };

        options.Kubernetes.Enable = true;
        options.Kubernetes.PathTokenKubernetes = AppDomain.CurrentDomain.BaseDirectory + "/token";

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
