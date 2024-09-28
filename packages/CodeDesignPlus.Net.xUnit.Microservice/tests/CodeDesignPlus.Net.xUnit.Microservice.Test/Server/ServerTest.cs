using CodeDesignPlus.Net.xUnit.Microservice.Server;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Server;

public class ServerTest(Server<Program> server) : ServerBase<Program>(server), IClassFixture<Server<Program>>
{
    [Fact]
    public async Task GetWeatherForecast_ReturnsWeatherForecast()
    {
        //Act
        var response = await Client.GetAsync("/weatherforecast");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        var data = Serializers.JsonSerializer.Deserialize<IEnumerable<Api.WeatherForecast>>(responseString);

        Assert.NotNull(data);
        Assert.NotEmpty(data);
    }
}
