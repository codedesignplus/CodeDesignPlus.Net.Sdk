namespace CodeDesignPlus.Net.Redis.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly RedisOptions RedisOptions = new()
    {
        Instances = new Dictionary<string, Instance>() {
            { "test", new Instance() {ConnectionString = "localhost:6379,ssl=false"} }
        }
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            Redis = RedisOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
