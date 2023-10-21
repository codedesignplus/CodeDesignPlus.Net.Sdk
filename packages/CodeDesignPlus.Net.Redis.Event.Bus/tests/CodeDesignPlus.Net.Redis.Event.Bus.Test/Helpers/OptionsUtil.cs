using CodeDesignPlus.Net.Event.Bus.Options;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers;

public static class OptionsUtil
{
    public static readonly RedisEventBusOptions RedisEventBusOptions = new()
    {
        Enable = true,
        Name = "test",
    };

    public static readonly EventBusOptions EventBusOptions = new()
    {
        Enable = true,
        Name = nameof(CodeDesignPlus.Net.Event.Bus.Options.EventBusOptions.Name),
        Email = "codedesignplus@outlook.com"
    };


    public static readonly object AppSettings = new
    {
        RedisEventBus = RedisEventBusOptions
    };

}
