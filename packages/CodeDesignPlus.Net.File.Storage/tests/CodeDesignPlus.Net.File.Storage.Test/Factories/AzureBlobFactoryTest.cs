using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.Security.Abstractions;
using O = Microsoft.Extensions.Options;
using Moq;
using Azure.Storage.Blobs;


namespace CodeDesignPlus.Net.File.Storage.Test.Factories
{
    public class AzureBlobFactoryTest
    {
        [Fact]
        public void Constructor_ValidOptionsAndUserContext_InitializesProperties()
        {
            // Arrange
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            var fileOptions = new FileStorageOptions();
            var options = O.Options.Create(fileOptions);

            // Act
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);

            // Assert
            Assert.Equal(fileOptions, factory.Options);
            Assert.Equal(userContextMock.Object, factory.UserContext);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            IOptions<FileStorageOptions> options = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object));
        }

        [Fact]
        public void Constructor_NullUserContext_ThrowsArgumentNullException()
        {
            // Arrange
            var fileOptions = new FileStorageOptions();
            var options = O.Options.Create(fileOptions);
            IUserContext<Guid, Guid> userContext = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AzureBlobFactory<Guid, Guid>(options, userContext));
        }

        [Fact]
        public void Create_WhenAzureBlobIsNotEnabled_ThrowsFileStorageException()
        {
            // Arrange
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            var fileOptions = new FileStorageOptions { AzureBlob = new() { Enable = false } };
            var options = O.Options.Create(fileOptions);
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);

            // Act & Assert
            var exception = Assert.Throws<FileStorageException>(() => factory.Create());

            Assert.Equal("The AzureBlob is not enable", exception.Message);
        }

        [Fact]
        public void Create_WhenClientIsNotNull_ReturnsItself()
        {
            // Arrange
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);

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
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            var fileOptions = new FileStorageOptions { AzureBlob = new() { Enable = true, UsePasswordLess = true, Uri = new Uri("https://account.blob.core.windows.net") } };
            var options = O.Options.Create(fileOptions);
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);

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
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);

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
            var userContextMock = new Mock<IUserContext<Guid, Guid>>();
            userContextMock.Setup(x => x.Tenant).Returns(tenant);

            var options = O.Options.Create(OptionsUtil.FileStorageOptions);
            var factory = new AzureBlobFactory<Guid, Guid>(options, userContextMock.Object);
            factory.Create();

            // Act
            var result = factory.GetContainerClient();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BlobContainerClient>(result);
            Assert.Equal(tenant.ToString(), result.Name);
        }
    }
}