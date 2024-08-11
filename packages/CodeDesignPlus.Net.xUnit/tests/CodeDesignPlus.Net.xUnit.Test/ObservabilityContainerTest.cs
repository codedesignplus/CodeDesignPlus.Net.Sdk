using CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.xUnit.Test;

public class ObservabilityContainerTest(ObservabilityContainer container) : IClassFixture<ObservabilityContainer>
{

    [Fact]
    public void ContainerNotNull()
    {
        Assert.NotNull(container);
    }

}
