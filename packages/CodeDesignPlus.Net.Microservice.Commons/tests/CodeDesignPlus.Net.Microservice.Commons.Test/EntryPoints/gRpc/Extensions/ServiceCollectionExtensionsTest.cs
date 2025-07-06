
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Extensions;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.gRpc.Extensions;

public class GrpcServiceCollectionExtensionsTest
{
    [Fact]
    public void AddGrpcInterceptors_ServiceCollectionIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services!.AddGrpcInterceptors());
    }

    [Fact]
    public void AddGrpcInterceptors_RegistersErrorInterceptorAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); 

        // Act
        services.AddGrpcInterceptors();
        var provider = services.BuildServiceProvider();

        // Assert
        var interceptor1 = provider.GetService<ErrorInterceptor>();
        var interceptor2 = provider.GetService<ErrorInterceptor>();

        Assert.NotNull(interceptor1);
        Assert.Same(interceptor1, interceptor2);
    }
}

