using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Cache.Test.Helpers;
using CodeDesignPlus.Net.Serializers;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Services;

public class RedisCacheManagerTest
{

    private readonly CoreOptions core ;

    private readonly IOptions<CoreOptions> coreOptions;

    public RedisCacheManagerTest()
    {
        this.core = new ()
        {
            AppName = "ms-test",
                Business = "CodeDesignPlus",
                Description = "Unit test",
                Version = "1",
                Contact = new Contact()
                {
                    Email = "codedesignplus@codedesignplus.com",
                    Name = "CodeDesignPlus"
                }
        };

        this.coreOptions = Microsoft.Extensions.Options.Options.Create(core);
    }

    [Fact]
    public async Task ClearAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.ClearAsync();

        // Assert
        loggerMock.VerifyLogging($"The cache will not be cleared because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task ClearAsync_DatabaseIsNotNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.ClearAsync();

        // Assert
        loggerMock.VerifyLogging($"The cache will be cleared", LogLevel.Warning, Times.Once());
        databaseMock.Verify(x => x.ExecuteAsync("FLUSHDB"), Times.Once());
    }

    [Fact]
    public async Task ExistsAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => cacheManager.ExistsAsync(null!));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
    }

    [Fact]
    public async Task ExistsAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var result = await cacheManager.ExistsAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} could not be verified because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_DatabaseIsNotNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var result = await cacheManager.ExistsAsync(expected);

        // Assert
        databaseMock.Verify(x => x.KeyExistsAsync(GetKey(expected), It.IsAny<CommandFlags>()), Times.Once());
        Assert.False(result);
    }

    [Fact]
    public async Task GetAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => cacheManager.GetAsync<string>(null!));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
    }

    [Fact]
    public async Task GetAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var result = await cacheManager.GetAsync<string>(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} could not be retrieved because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_DatabaseIsNotNullAndDataIsNull_ReturnValueFromCache()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var result = await cacheManager.GetAsync<string>(expected);

        // Assert
        databaseMock.Verify(x => x.StringGetAsync(GetKey(expected), It.IsAny<CommandFlags>()), Times.Once());
        loggerMock.VerifyLogging($"The key {GetKey(expected)} does not exist in the cache", LogLevel.Debug, Times.Once());
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(CacheTestData))]
    public async Task GetAsync_MultipleValues_ReturnValueFromCache(object data)
    {
        // Arrange
        var serialized = JsonSerializer.Serialize(data);
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);
        databaseMock.Setup(x => x.StringGetAsync(GetKey(expected), It.IsAny<CommandFlags>())).ReturnsAsync(serialized);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var method = typeof(RedisCacheManager).GetMethod("GetAsync");
        var generic = method!.MakeGenericMethod(data.GetType());
        dynamic awaitable = generic.Invoke(cacheManager, [expected])!;
        await awaitable;
        var result = awaitable.GetAwaiter().GetResult();

        // Assert
        databaseMock.Verify(x => x.StringGetAsync(GetKey(expected), It.IsAny<CommandFlags>()), Times.Once());
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task RemoveAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => cacheManager.RemoveAsync(null!));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
    }

    [Fact]
    public async Task RemoveAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.RemoveAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} could not be removed because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task RemoveAsync_DatabaseIsNotNull_RemoveKeyFromCache()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.RemoveAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} will be removed from the cache", LogLevel.Debug, Times.Once());
        databaseMock.Verify(x => x.KeyDeleteAsync(GetKey(expected), It.IsAny<CommandFlags>()), Times.Once());
    }

    [Fact]
    public async Task SetAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => cacheManager.SetAsync(null!, "value"));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'key')", exception.Message);
    }

    [Fact]
    public async Task SetAsync_ValueIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => cacheManager.SetAsync<string>(Guid.NewGuid().ToString(), null!));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'value')", exception.Message);
    }

    [Fact]
    public async Task SetAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.SetAsync(expected, "value");

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} could not be stored because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
    }

    [Theory]
    [ClassData(typeof(CacheTestData))]
    public async Task SetAsync_DatabaseIsNotNull_SetValueInCache(object data)
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var redisCacheOptions = new RedisCacheOptions()
        {
            Enable = true,
            Expiration = TimeSpan.FromMinutes(5)
        };
        var options = Microsoft.Extensions.Options.Options.Create(redisCacheOptions);
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        await cacheManager.SetAsync(expected, data);

        // Assert
        loggerMock.VerifyLogging($"The key {GetKey(expected)} will be stored in the cache for {redisCacheOptions.Expiration.TotalSeconds} seconds", LogLevel.Debug, Times.Once());
        databaseMock.Verify(x => x.StringSetAsync(GetKey(expected), JsonSerializer.Serialize(data), redisCacheOptions.Expiration, false, When.Always, CommandFlags.None), Times.Once());
    }

    [Fact]
    public async Task SetGlobalAsync_KeyIsNotNamespacedByApplication()
    {
        // Arrange
        var key = "CodeDesignPlus:shared:Tenant:" + Guid.NewGuid();
        var data = "Acme Corp";
        var (cacheManager, databaseMock, _, redisCacheOptions) = BuildCacheManager();

        // Act
        await cacheManager.SetGlobalAsync(key, data);

        // Assert
        databaseMock.Verify(x => x.StringSetAsync(key, JsonSerializer.Serialize(data), redisCacheOptions.Expiration, false, When.Always, CommandFlags.None), Times.Once());
        databaseMock.Verify(x => x.StringSetAsync(GetKey(key), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never());
    }

    [Fact]
    public async Task GetGlobalAsync_KeyExists_ReturnsDeserializedValue()
    {
        // Arrange
        var key = "CodeDesignPlus:shared:Tenant:" + Guid.NewGuid();
        var data = "Acme Corp";
        var (cacheManager, databaseMock, _, _) = BuildCacheManager();

        databaseMock.Setup(x => x.StringGetAsync(key, CommandFlags.None)).ReturnsAsync(JsonSerializer.Serialize(data));

        // Act
        var result = await cacheManager.GetGlobalAsync<string>(key);

        // Assert
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task GetGlobalAsync_KeyDoesNotExist_ReturnsDefault()
    {
        // Arrange
        var key = "CodeDesignPlus:shared:Tenant:" + Guid.NewGuid();
        var (cacheManager, databaseMock, _, _) = BuildCacheManager();

        databaseMock.Setup(x => x.StringGetAsync(key, CommandFlags.None)).ReturnsAsync(RedisValue.Null);

        // Act
        var result = await cacheManager.GetGlobalAsync<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GlobalSet_AddRemoveAndRead_UseTheRawKey()
    {
        // Arrange
        const string key = "CodeDesignPlus:shared:Tenants:Active";
        var tenant = Guid.NewGuid().ToString();
        var (cacheManager, databaseMock, _, _) = BuildCacheManager();

        databaseMock.Setup(x => x.SetMembersAsync(key, CommandFlags.None)).ReturnsAsync([tenant]);

        // Act
        await cacheManager.AddToGlobalSetAsync(key, tenant);
        await cacheManager.RemoveFromGlobalSetAsync(key, tenant);
        var members = await cacheManager.GetGlobalSetMembersAsync(key);

        // Assert
        databaseMock.Verify(x => x.SetAddAsync(key, tenant, CommandFlags.None), Times.Once());
        databaseMock.Verify(x => x.SetRemoveAsync(key, tenant, CommandFlags.None), Times.Once());
        Assert.Equal([tenant], members);
    }

    [Fact]
    public async Task GlobalOperations_DatabaseIsNull_DegradeWithoutThrowing()
    {
        // Arrange
        const string key = "CodeDesignPlus:shared:Tenants:Active";
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        // Act
        var value = await cacheManager.GetGlobalAsync<string>(key);
        var members = await cacheManager.GetGlobalSetMembersAsync(key);
        await cacheManager.SetGlobalAsync(key, "Acme Corp");
        await cacheManager.RemoveGlobalAsync(key);
        await cacheManager.AddToGlobalSetAsync(key, Guid.NewGuid().ToString());
        await cacheManager.RemoveFromGlobalSetAsync(key, Guid.NewGuid().ToString());

        // Assert
        Assert.Null(value);
        Assert.Empty(members);
    }

    private (RedisCacheManager, Mock<IDatabase>, Mock<ILogger<RedisCacheManager>>, RedisCacheOptions) BuildCacheManager()
    {
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var redisCacheOptions = new RedisCacheOptions();
        var options = Microsoft.Extensions.Options.Options.Create(redisCacheOptions);
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisServiceMock = new Mock<Redis.Abstractions.IRedis>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options, coreOptions);

        return (cacheManager, databaseMock, loggerMock, redisCacheOptions);
    }

    private string GetKey(string key)
    {
        return $"{core.Business}:{core.AppName}:{key}";
    }
}
