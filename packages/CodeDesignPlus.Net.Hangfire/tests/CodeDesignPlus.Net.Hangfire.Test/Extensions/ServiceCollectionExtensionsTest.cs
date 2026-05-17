using CodeDesignPlus.Net.Redis.Abstractions;

namespace CodeDesignPlus.Net.Hangfire.Test.Extensions;

/// <summary>
/// Pruebas unitarias para <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public class ServiceCollectionExtensionsTest
{
    /// <summary>
    /// Verifica que <see cref="ServiceCollectionExtensions.AddHangfire{TProgram}"/>
    /// lanza <see cref="ArgumentNullException"/> cuando services es nulo.
    /// </summary>
    [Fact]
    public void AddHangfire_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddHangfire<Program>(services, Mock.Of<IConfiguration>()));
    }

    /// <summary>
    /// Verifica que <see cref="ServiceCollectionExtensions.AddHangfire{TProgram}"/>
    /// lanza <see cref="ArgumentNullException"/> cuando configuration es nulo.
    /// </summary>
    [Fact]
    public void AddHangfire_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddHangfire<Program>(services, null!));
    }

    /// <summary>
    /// Verifica que cuando Hangfire está deshabilitado (<c>Enable = false</c>),
    /// no se registra ningún servicio de Hangfire.
    /// </summary>
    [Fact]
    public void AddHangfire_DoesNotRegister_WhenDisabled()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            [$"{HangfireOptions.Section}:Enable"] = "false"
        });

        // Act
        services.AddHangfire<Program>(config);

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IJobService));
    }

    /// <summary>
    /// Verifica que cuando Hangfire está habilitado, se registra <see cref="IJobService"/>.
    /// </summary>
    [Fact]
    public void AddHangfire_RegistersIJobService_WhenEnabled()
    {
        // Arrange
        var services = new ServiceCollection();

        // Se necesita un IRedisFactory mockeado para que AddHangfire no falle al construir el container
        var redisFactoryMock = new Mock<IRedisFactory>();
        var redisMock = new Mock<IRedis>();
        var connectionMock = new Mock<StackExchange.Redis.IConnectionMultiplexer>();
        redisMock.Setup(r => r.Connection).Returns(connectionMock.Object);
        redisFactoryMock.Setup(f => f.Create(FactoryConst.RedisCore)).Returns(redisMock.Object);
        services.AddSingleton(redisFactoryMock.Object);

        var config = BuildConfig(new Dictionary<string, string?>
        {
            [$"{HangfireOptions.Section}:Enable"] = "true",
            [$"{HangfireOptions.Section}:Prefix"] = "hangfire:test:"
        });

        // Act
        services.AddHangfire<Program>(config);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IJobService));
    }

    /// <summary>
    /// Verifica que <see cref="ServiceCollectionExtensions.UseHangfireDashboard{TProgram}"/>
    /// lanza <see cref="ArgumentNullException"/> cuando app es nulo.
    /// </summary>
    [Fact]
    public void UseHangfireDashboard_ThrowsArgumentNullException_WhenAppIsNull()
    {
        Microsoft.AspNetCore.Builder.IApplicationBuilder app = null!;

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.UseHangfireDashboard<Program>(app, Mock.Of<IConfiguration>()));
    }

    /// <summary>
    /// Verifica que <see cref="ServiceCollectionExtensions.UseHangfireDashboard{TProgram}"/>
    /// lanza <see cref="ArgumentNullException"/> cuando configuration es nulo.
    /// </summary>
    [Fact]
    public void UseHangfireDashboard_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        var app = new Mock<Microsoft.AspNetCore.Builder.IApplicationBuilder>().Object;

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.UseHangfireDashboard<Program>(app, null!));
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static IConfiguration BuildConfig(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    /// <summary>
    /// Clase marcadora usada como TProgram en las pruebas.
    /// </summary>
    public class Program { }
}
