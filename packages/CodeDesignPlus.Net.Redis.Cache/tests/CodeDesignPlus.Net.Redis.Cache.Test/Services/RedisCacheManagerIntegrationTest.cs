using System;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Cache.Test.Helpers;
using CodeDesignPlus.Net.xUnit.Containers.RedisContainer;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Services;

public class RedisCacheManagerIntegrationTest(RedisContainer fixture) : IClassFixture<RedisContainer>
{
    [Fact]
    public async Task SetAsync_GetAsync_Success()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var value = Guid.NewGuid();
        var redisService = fixture.RedisServer;

        var redisFactory = new Mock<IRedisFactory>();
        redisFactory.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisService);

        var redisCacheManager = new RedisCacheManager(redisFactory.Object, Mock.Of<ILogger<RedisCacheManager>>(), O.Options.Create(new RedisCacheOptions()));

        // Act
        await redisCacheManager.SetAsync(expected, value, TimeSpan.FromMinutes(5));

        var result = await redisCacheManager.GetAsync<Guid>(expected);

        // Assert
        Assert.Equal(value.ToString(), result.ToString());
    }

    [Fact]
    public async Task SetAsync_ExistAsync_Success()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var value = Guid.NewGuid();
        var redisService = fixture.RedisServer;

        var redisFactory = new Mock<IRedisFactory>();
        redisFactory.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisService);

        var redisCacheManager = new RedisCacheManager(redisFactory.Object, Mock.Of<ILogger<RedisCacheManager>>(), O.Options.Create(new RedisCacheOptions()));

        // Act
        await redisCacheManager.SetAsync(expected, value, TimeSpan.FromMinutes(5));

        var exist = await redisCacheManager.ExistsAsync(expected);

        // Assert
        Assert.True(exist);
    }
    
    [Fact]
    public async Task SetAsync_RemoveAsync_Success()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var value = Guid.NewGuid();
        var redisService = fixture.RedisServer;

        var redisFactory = new Mock<IRedisFactory>();
        redisFactory.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisService);

        var redisCacheManager = new RedisCacheManager(redisFactory.Object, Mock.Of<ILogger<RedisCacheManager>>(), O.Options.Create(new RedisCacheOptions()));

        // Act
        await redisCacheManager.SetAsync(expected, value, TimeSpan.FromMinutes(5));

        await redisCacheManager.RemoveAsync(expected);

        var exist = await redisCacheManager.ExistsAsync(expected);

        // Assert
        Assert.False(exist);
    }

    [Fact]
     public async Task SetAsync_ClearAsync_Success()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var value = Guid.NewGuid();
        var redisService = fixture.RedisServer;

        var redisFactory = new Mock<IRedisFactory>();
        redisFactory.Setup(x => x.Create(FactoryConst.RedisCore)).Returns(redisService);

        var redisCacheManager = new RedisCacheManager(redisFactory.Object, Mock.Of<ILogger<RedisCacheManager>>(), O.Options.Create(new RedisCacheOptions()));

        // Act
        await redisCacheManager.SetAsync(expected, value, TimeSpan.FromMinutes(5));

        await redisCacheManager.ClearAsync();
        
        var exist = await redisCacheManager.ExistsAsync(expected);

        // Assert
        Assert.False(exist);
    }
}
