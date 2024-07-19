using CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

namespace CodeDesignPlus.Net.xUnit.Test;

public class ObservabilityContainerTest(ObservabilityContainer container) : IClassFixture<ObservabilityContainer>
{
    // [Fact]
    // public async Task CheckOpenTelemetryCollector()
    // {
    //     using var httpClient = new HttpClient();

    //     var response = await httpClient.GetAsync("http://localhost:55680");

    //     Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    // }

    [Fact]
    public async Task CheckLoki()
    {
        using var httpClient = new HttpClient();

        var response = await httpClient.GetAsync("http://localhost:3100/loki/api/v1/health");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
