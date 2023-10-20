namespace CodeDesignPlus.Net.Redis.Test.Helpers;

public static class OptionsUtil
{
    public static readonly RedisOptions RedisOptions = new()
    {
        Instances = new Dictionary<string, Instance>() {
            { "test", new Instance() {ConnectionString = "localhost:6379,ssl=false"} }
        }
    };

    public static readonly object AppSettings = new
    {
        Redis = RedisOptions
    };

}
