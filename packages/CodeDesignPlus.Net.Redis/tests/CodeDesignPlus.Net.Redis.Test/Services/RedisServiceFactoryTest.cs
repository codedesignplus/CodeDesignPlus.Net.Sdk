using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Redis.Test.Services;

public class RedisServiceFactoryTests
{
    private readonly Mock<IServiceProvider> mockServiceProvider;
    private readonly Mock<IOptions<RedisOptions>> mockOptions;
    private readonly Mock<IRedisService> mockRedisService;
    private readonly Mock<ILogger<RedisServiceFactory>> mockLogger;

    public RedisServiceFactoryTests()
    {
        this.mockServiceProvider = new Mock<IServiceProvider>();
        this.mockOptions = new Mock<IOptions<RedisOptions>>();
        this.mockRedisService = new Mock<IRedisService>();
        this.mockLogger = new Mock<ILogger<RedisServiceFactory>>();
    }


    [Fact]
    public void Constructor_WhenServiceProviderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider nullServiceProvider = null!;
        var options = O.Options.Create(new RedisOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisServiceFactory(nullServiceProvider, options, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenOptionsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisServiceFactory(serviceProvider, null, mockLogger.Object));
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(new RedisOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisServiceFactory(serviceProvider, options, null));
    }

    [Fact]
    public void Constructor_WhenArgumentsAreValid_DoesNotThrow()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(new RedisOptions());

        // Act & Assert
        var exception = Record.Exception(() => new RedisServiceFactory(serviceProvider, options, mockLogger.Object));

        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_Initialize_Susesfully()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(new RedisOptions());

        // Act
        var factory = new RedisServiceFactory(serviceProvider, options, mockLogger.Object);

        // Assert
        Assert.NotNull(factory);
        mockLogger.VerifyLogging("RedisServiceFactory has been initialized", LogLevel.Information, Times.Once());
    }

    [Fact]
    public void Create_WhenNameIsNull_ThrowsArgumentNullException()
    {
        var factory = new RedisServiceFactory(this.mockServiceProvider.Object, O.Options.Create(new RedisOptions()), mockLogger.Object);

        Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Fact]
    public void Create_WhenNameIsEmpty_ThrowsArgumentNullException()
    {
        var factory = new RedisServiceFactory(this.mockServiceProvider.Object, O.Options.Create(new RedisOptions()), mockLogger.Object);

        Assert.Throws<ArgumentNullException>(() => factory.Create(string.Empty));
    }

    [Fact]
    public void Create_WhenInstanceNotRegistered_ThrowsRedisException()
    {
        // Setup
        var redisOptions = new RedisOptions
        {
            Instances = [] // Empty instances
        };
        this.mockOptions.Setup(opt => opt.Value).Returns(redisOptions);
        var factory = new RedisServiceFactory(this.mockServiceProvider.Object, this.mockOptions.Object, mockLogger.Object);

        // Assert
        Assert.Throws<RedisException>(() => factory.Create("InstanceName"));
    }

    [Fact]
    public void Create_WhenInstanceIsRegistered_ReturnsIRedisService()
    {
        // Setup        
        var instanceExpected = new Instance()
        {
            ConnectionString = "localhost:6379"
        };

        var redisService = new Mock<IRedisService>();

        redisService
            .Setup(x => x.Initialize(It.IsAny<Instance>()))
            .Callback<Instance>((instance) =>
            {
                Assert.Equal(instanceExpected, instance);
            });

        var redisOptions = new RedisOptions
        {
            Instances = new Dictionary<string, Instance>
            {
                { "InstanceName", instanceExpected }
            }
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(x => redisService.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = O.Options.Create(redisOptions);

        var factory = new RedisServiceFactory(serviceProvider, options, mockLogger.Object);

        // Act
        var service = factory.Create("InstanceName");
        var service2 = factory.Create("InstanceName");

        // Assert
        Assert.NotNull(service);
        Assert.Equal(service, service2);
        Assert.Equal(redisService.Object, service);

        mockLogger.VerifyLogging("Redis instance InstanceName has been added to the factory", LogLevel.Information, Times.Once());
    }


}