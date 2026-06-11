using Azure.Storage.Files.Shares;
using CodeDesignPlus.Net.File.Storage.Factories;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Factories
{
    public class AzureFileFactoryTest
    {
        [Fact]
        public void Constructor_ValidOptions_InitializesProperties()
        {
            // Arrange
            var optionsMock = new Mock<IOptions<FileStorageOptions>>();
            var optionsValue = new FileStorageOptions();
            optionsMock.Setup(o => o.Value).Returns(optionsValue);

            // Act
            var factory = new AzureFileFactory(optionsMock.Object);

            // Assert
            Assert.Same(optionsValue, factory.Options);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            IOptions<FileStorageOptions> options = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AzureFileFactory(options));
        }

        [Fact]
        public void Create_WhenAzureFileIsNotEnabled_ThrowsFileStorageException()
        {
            // Arrange
            var optionsMock = new Mock<IOptions<FileStorageOptions>>();
            var optionsValue = new FileStorageOptions { AzureFile = new() { Enable = false } };
            optionsMock.Setup(o => o.Value).Returns(optionsValue);

            var factory = new AzureFileFactory(optionsMock.Object);

            // Act & Assert
            Assert.Equal(factory, factory.Create());
        }

        [Fact]
        public void Create_WhenClientIsNotNull_ReturnsItself()
        {
            // Arrange
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);

            var factory = new AzureFileFactory(options);

            // Act
            var result1 = factory.Create();
            var result2 = factory.Create();

            // Assert
            Assert.Same(factory, result1);
            Assert.Same(factory, result2);
            Assert.Same(result1, result2);
        }

        [Fact]
        public void Create_WhenUsePasswordLessIsTrue_CreatesClientWithUriAndDefaultAzureCredential()
        {
            // Arrange
            var optionsValue = new FileStorageOptions { AzureFile = new() { Enable = true, UsePasswordLess = true, Uri = new Uri("https://example.com") } };
            var options = O.Options.Create(optionsValue);

            var factory = new AzureFileFactory(options);

            // Act
            var result = factory.Create();

            // Assert
            Assert.NotNull(factory.Client);
            Assert.Equal(factory, result);
            Assert.Equal("https://example.com/", factory.Client.Uri.ToString());
            Assert.False(factory.Client.CanGenerateAccountSasUri);
        }

        [Fact]
        public void Create_WhenUsePasswordLessIsFalse_CreatesClientWithConnectionString()
        {
            // Arrange
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);

            var factory = new AzureFileFactory(options);

            // Act
            var result = factory.Create();

            // Assert
            Assert.NotNull(factory.Client);
            Assert.Equal(factory, result);
            Assert.IsType<ShareServiceClient>(factory.Client);
            Assert.Equal(OptionsUtil.FileStorageOptions.AzureFile.AccountName, factory.Client.AccountName);
            Assert.True(factory.Client.CanGenerateAccountSasUri);
        }


        [Fact]
        public void GetContainerClient_Called_Success()
        {
            // Arrange
            var tenant = Guid.NewGuid();

            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureFileFactory(options);
            factory.Create();

            // Act
            var result = factory.GetContainerClient(tenant);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ShareClient>(result);
            Assert.Equal(tenant.ToString(), result.Name);
        }
    }
}
