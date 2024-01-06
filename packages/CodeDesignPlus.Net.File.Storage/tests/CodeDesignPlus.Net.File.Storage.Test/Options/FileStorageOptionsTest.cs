using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.File.Storage.Test.Options;

public class FileStorageOptionsTest
{
    [Fact]
    public void FileStorageOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void FileStorageOptions_UriDownload_Required()
    {
        // Arrange
        var options = new FileStorageOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Single(results);
        Assert.Equal("The UriDownload field is required.", results.First().ErrorMessage);
    }

    [Fact]
    public void AzureBlobOptions_Enable_Required()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Equal(TypeProviders.AzureBlobProvider, AzureBlobOptions.TypeProvider);
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The DefaultEndpointsProtocol is required");
        Assert.Contains(results, x => x.ErrorMessage == "The AccountName is required");
        Assert.Contains(results, x => x.ErrorMessage == "The AccountKey is required");
        Assert.Contains(results, x => x.ErrorMessage == "The EndpointSuffix is required");
    }

    [Fact]
    public void AzureBlobOptions_UsePasswordLess_Required()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
                UsePasswordLess = true
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The Uri is required");
    }

    [Fact]
    public void AzureBlobOptions_Values_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
                DefaultEndpointsProtocol = "https",
                AccountName = "AccountName",
                AccountKey = "AccountKey",
                EndpointSuffix = "core.windows.net",
                Uri = new Uri("https://localhost:5001")
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void AzureBlobOptions_Disable_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureBlob = new AzureBlobOptions()
            {
                Enable = false,
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void AzureFileOptions_Enable_Required()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Equal(TypeProviders.AzureFileProvider, AzureFileOptions.TypeProvider);
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The DefaultEndpointsProtocol is required");
        Assert.Contains(results, x => x.ErrorMessage == "The AccountName is required");
        Assert.Contains(results, x => x.ErrorMessage == "The AccountKey is required");
        Assert.Contains(results, x => x.ErrorMessage == "The EndpointSuffix is required");
    }

    [Fact]
    public void AzureFileOptions_UsePasswordLess_Required()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
                UsePasswordLess = true
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The Uri is required");
    }

    [Fact]
    public void AzureFileOptions_Values_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
                DefaultEndpointsProtocol = "https",
                AccountName = "AccountName",
                AccountKey = "AccountKey",
                EndpointSuffix = "core.windows.net",
                Uri = new Uri("https://localhost:5001")
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void AzureFileOptions_Disable_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            AzureFile = new AzureFileOptions()
            {
                Enable = false,
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void LocalOptions_Enable_Required()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            Local = new LocalOptions()
            {
                Enable = true
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Equal(TypeProviders.LocalProvider, LocalOptions.TypeProvider);
        Assert.Equal("The Folder property is required.", results.First().ErrorMessage);
    }

    [Fact]
    public void LocalOptions_Values_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            Local = new LocalOptions()
            {
                Enable = true,
                Folder = "C:\\Temp"
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void LocalOptions_Disable_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            Local = new LocalOptions()
            {
                Enable = false
            },
            UriDownload = new Uri("https://localhost:5001/api/v1/file-storage/download")
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
