using CodeDesignPlus.Net.xUnit.Microservice.Server;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Server;

public class ServerTest : ServerBase<Program>, IClassFixture<Server<Program>>
{
    private readonly Server<Program> server;

    private readonly Guid value = Guid.NewGuid();

    public ServerTest(Server<Program> server) : base(server)
    {
        this.server = server;

        server.InMemoryCollection = (x) =>
        {
            x.Add("Custom:Item", value.ToString());
        };
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsWeatherForecast()
    {
        //Act
        var response = await Client.GetAsync("/weatherforecast");

        var value = server.Services.GetService<IConfiguration>()?.GetValue<string>("Custom:Item");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        var data = Serializers.JsonSerializer.Deserialize<IEnumerable<Api.WeatherForecast>>(responseString);

        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Equal(value, this.value.ToString());
    }
}
