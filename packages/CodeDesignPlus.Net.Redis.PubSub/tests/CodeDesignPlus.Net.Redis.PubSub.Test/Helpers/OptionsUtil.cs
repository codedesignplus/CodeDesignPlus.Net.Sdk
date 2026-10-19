using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Options;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers;

public static class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "ms-redis-test",
        TypeEntryPoint = "rest",
        Business = "CodeDesignPlus",
        Description = "Test project for CodeDesignPlus.Net.Redis.PubSub",
        Version = "v1",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "wliscano@codedesignplus.com"
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

    public static RedisPubSubOptions RedisPubSubOptions() => new()
    {
        Enable = true,
        EnableDiagnostic = true,
        RegisterAutomaticHandlers = true
    };

    public static object AppSettings => new
    {
        Core = CoreOptions,
        Redis = RedisOptions,
        RedisPubSub = RedisPubSubOptions()
    };
}
