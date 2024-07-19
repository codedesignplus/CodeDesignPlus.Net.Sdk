using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Options;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers;

public static class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "CodeDesignPlus.Net.Redis.PubSub.Test",
        Business = "CodeDesignPlus",
        Description = "Test project for CodeDesignPlus.Net.Redis.PubSub",
        Version = "v1",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };

    public static RedisOptions RedisOptions => new()
    {
        Instances = new Dictionary<string, Instance>()
        {
            {
                "Core", new Instance()
                {
                    ConnectionString = "localhost:6379"
                }
            }
        }
    };

    public static RedisPubSubOptions RedisPubSubOptions(bool useQueue) => new()
    {
        Enable = true,
        UseQueue = useQueue,
        EnableDiagnostic = true,
        RegisterAutomaticHandlers = true,
        SecondsWaitQueue = 5
    };

    public static object AppSettings => new
    {
        Core = CoreOptions,
        Redis = RedisOptions,
        RedisPubSub = RedisPubSubOptions(true)
    };
}
