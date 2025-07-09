using CodeDesignPlus.Net.Microservice.Commons.MediatR;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.MediatR;

public class MediatRExtensionsTest
{
    [Fact]
    public void AddMediatRR_ShouldRegisterMediatRServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLogging();
        services.AddMediatR<Startup>();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var requestsHandler = services.Where(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<>));
        var queriesHandler = services.Where(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        var pipelineBehavior = services.FirstOrDefault(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        var mediatRService = serviceProvider.GetService<IMediator>();
        Assert.NotNull(mediatRService);
        Assert.NotEmpty(requestsHandler);
        Assert.NotEmpty(queriesHandler);
        Assert.NotNull(pipelineBehavior);
    }

}
