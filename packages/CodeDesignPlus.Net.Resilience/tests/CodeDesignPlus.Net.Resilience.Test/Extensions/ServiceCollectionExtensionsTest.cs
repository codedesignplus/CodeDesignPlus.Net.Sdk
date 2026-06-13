using CodeDesignPlus.Net.Resilience.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CodeDesignPlus.Net.Resilience.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddResilience_ServiceCollectionIsNull_ThrowsArgumentNullException()
    {
        ServiceCollection serviceCollection = null;

        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddResilience());

        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddResilience_ValidServiceCollection_ReturnsServices()
    {
        var serviceCollection = new ServiceCollection();

        var result = serviceCollection.AddResilience();

        Assert.NotNull(result);
        Assert.Same(serviceCollection, result);
    }
}
