using Azure.Storage.Blobs;
using CodeDesignPlus.Net.File.Storage.Factories;
using O = Microsoft.Extensions.Options;


namespace CodeDesignPlus.Net.File.Storage.Test.Factories
{
    public class AzureBlobFactoryTest
    {
        [Fact]
        public void Constructor_ValidOptions_InitializesProperties()
        {
            // Arrange
            var fileOptions = new FileStorageOptions();
            var options = O.Options.Create(fileOptions);

            // Act
            var factory = new AzureBlobFactory(options);

            // Assert
            Assert.Equal(fileOptions, factory.Options);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            IOptions<FileStorageOptions> options = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AzureBlobFactory(options));
        }

        [Fact]
        public void Create_WhenAzureBlobIsNotEnabled_ReturnSame()
        {
            // Arrange
            var fileOptions = new FileStorageOptions { AzureBlob = new() { Enable = false } };
            var options = O.Options.Create(fileOptions);
            var factory = new AzureBlobFactory(options);

            // Act & Assert
            Assert.Equal(factory, factory.Create());
        }

        [Fact]
        public void Create_WhenClientIsNotNull_ReturnsItself()
        {
            // Arrange
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory(options);

            // Act
            var factory1 = factory.Create();
            var factory2 = factory.Create();

            // Assert
            Assert.Equal(factory, factory1);
            Assert.Equal(factory, factory2);
            Assert.Equal(factory1, factory2);
        }

        [Fact]
        public void Create_WhenUsePasswordLessIsTrue_CreatesClientWithUriAndDefaultAzureCredential()
        {
            // Arrange
            var fileOptions = new FileStorageOptions { AzureBlob = new() { Enable = true, UsePasswordLess = true, Uri = new Uri("https://account.blob.core.windows.net") } };
            var options = O.Options.Create(fileOptions);
            var factory = new AzureBlobFactory(options);

            // Act
            var result = factory.Create();

            // Assert
            Assert.NotNull(factory.Client);
            Assert.IsType<BlobServiceClient>(factory.Client);
            Assert.Equal(fileOptions.AzureBlob.Uri.ToString(), factory.Client.Uri.ToString());
            Assert.False(factory.Client.CanGenerateAccountSasUri);
        }

        [Fact]
        public void Create_WhenUsePasswordLessIsFalse_CreatesClientWithConnectionString()
        {
            // Arrange
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory(options);

            // Act
            var result = factory.Create();

            // Assert
            Assert.NotNull(result.Client);
            Assert.Equal(factory, result);
            Assert.IsType<BlobServiceClient>(result.Client);
            Assert.Equal(OptionsUtil.FileStorageOptions.AzureBlob.AccountName, result.Client.AccountName);
            Assert.True(factory.Client.CanGenerateAccountSasUri);
        }

        [Fact]
        public void GetContainerClient_Called_Success()
        {
            // Arrange
            var tenant = Guid.NewGuid();

            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory(options);
            factory.Create();

            // Act
            var result = factory.GetContainerClient(tenant);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BlobContainerClient>(result);
            Assert.Equal(tenant.ToString(), result.Name);
        }
    }
}
