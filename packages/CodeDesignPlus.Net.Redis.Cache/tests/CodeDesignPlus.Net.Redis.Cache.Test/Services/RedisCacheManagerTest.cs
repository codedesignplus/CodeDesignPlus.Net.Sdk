using System.Reflection;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Cache.Services;
using CodeDesignPlus.Net.Redis.Cache.Test.Helpers;
using CodeDesignPlus.Net.Serializers;
using CodeDesignPlus.Net.xUnit.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Services;

public class RedisCacheManagerTest
{
    [Fact]
    public async Task ClearAsync_DatabaseIsNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        var result = await cacheManager.ExistsAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {expected} could not be verified because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_DatabaseIsNotNull_WriteWarning()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        var result = await cacheManager.ExistsAsync(expected);

        // Assert
        databaseMock.Verify(x => x.KeyExistsAsync(expected, It.IsAny<CommandFlags>()), Times.Once());
        Assert.False(result);
    }

    [Fact]
    public async Task GetAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        var result = await cacheManager.GetAsync<string>(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {expected} could not be retrieved because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_DatabaseIsNotNullAndDataIsNull_ReturnValueFromCache()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        var result = await cacheManager.GetAsync<string>(expected);

        // Assert
        databaseMock.Verify(x => x.StringGetAsync(expected, It.IsAny<CommandFlags>()), Times.Once());
        loggerMock.VerifyLogging($"The key {expected} does not exist in the cache", LogLevel.Debug, Times.Once());
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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);
        databaseMock.Setup(x => x.StringGetAsync(expected, It.IsAny<CommandFlags>())).ReturnsAsync(serialized);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        var method = typeof(RedisCacheManager).GetMethod("GetAsync");
        var generic = method!.MakeGenericMethod(data.GetType());
        dynamic awaitable = generic.Invoke(cacheManager, [expected])!;
        await awaitable;
        var result = awaitable.GetAwaiter().GetResult();

        // Assert
        databaseMock.Verify(x => x.StringGetAsync(expected, It.IsAny<CommandFlags>()), Times.Once());
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task RemoveAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        await cacheManager.RemoveAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {expected} could not be removed because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task RemoveAsync_DatabaseIsNotNull_RemoveKeyFromCache()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        await cacheManager.RemoveAsync(expected);

        // Assert
        loggerMock.VerifyLogging($"The key {expected} will be removed from the cache", LogLevel.Debug, Times.Once());
        databaseMock.Verify(x => x.KeyDeleteAsync(expected, It.IsAny<CommandFlags>()), Times.Once());
    }

    [Fact]
    public async Task SetAsync_KeyIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisCacheOptions());
        var redisFactoryMock = new Mock<IRedisServiceFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

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
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();

        redisServiceMock.SetupGet(x => x.Database).Returns((IDatabaseAsync)null!);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        await cacheManager.SetAsync(expected, "value");

        // Assert
        loggerMock.VerifyLogging($"The key {expected} could not be stored because the connection to the Redis server could not be established", LogLevel.Warning, Times.Once());
    }

    [Theory]
    [ClassData(typeof(CacheTestData))]
    public async Task SetAsync_DatabaseIsNotNull_SetValueInCache(object data)
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var loggerMock = new Mock<ILogger<RedisCacheManager>>();
        var redisCacheOptions = new RedisCacheOptions() {
            Enable = true,
            Expiration = TimeSpan.FromMinutes(5)
        };
        var options = Microsoft.Extensions.Options.Options.Create(redisCacheOptions);
        var redisFactoryMock = new Mock<IRedisServiceFactory>();
        var redisServiceMock = new Mock<IRedisService>();
        var databaseMock = new Mock<IDatabase>();

        redisServiceMock.SetupGet(x => x.Database).Returns(databaseMock.Object);
        redisFactoryMock.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisServiceMock.Object);

        var cacheManager = new RedisCacheManager(redisFactoryMock.Object, loggerMock.Object, options);

        // Act
        await cacheManager.SetAsync(expected, data);

        // Assert
        loggerMock.VerifyLogging($"The key {expected} will be stored in the cache for {redisCacheOptions.Expiration.TotalSeconds} seconds", LogLevel.Debug, Times.Once());
        databaseMock.Verify(x => x.StringSetAsync(expected, JsonSerializer.Serialize(data), redisCacheOptions.Expiration, false, When.Always, CommandFlags.None), Times.Once());
    }
}
