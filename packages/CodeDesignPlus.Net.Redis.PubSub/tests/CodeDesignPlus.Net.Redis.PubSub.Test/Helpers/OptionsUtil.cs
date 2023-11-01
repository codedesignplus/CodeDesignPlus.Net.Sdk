using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers;

public static class OptionsUtil
{
    public static readonly RedisPubSubOptions RedisPubSubOptions = new()
    {
        Enable = true,
        Name = "test",
    };

    public static readonly PubSubOptions PubSubOptions = new()
    {
        EnableQueue = true,
    };


    public static readonly object AppSettings = new
    {
        RedisPubSub = RedisPubSubOptions
    };

}
