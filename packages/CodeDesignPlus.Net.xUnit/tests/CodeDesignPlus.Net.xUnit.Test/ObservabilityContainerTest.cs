using CodeDesignPlus.Net.xUnit.Containers.ObservabilityContainer;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(ObservabilityCollectionFixture.Collection)]
public class ObservabilityContainerTest(ObservabilityCollectionFixture fixture) 
{

    [Fact]
    public void CheckRunning()
    {
        Assert.True(fixture.Container.IsRunning);
    }

}
