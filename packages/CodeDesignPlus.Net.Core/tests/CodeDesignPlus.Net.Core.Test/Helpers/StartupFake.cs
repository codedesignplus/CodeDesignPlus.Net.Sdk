using System;

namespace CodeDesignPlus.Net.Core.Test.Helpers;

public class StartupFake: IStartup
{
    public static bool Initialized { get; private set; }

    public void Initialize(IServiceCollection services, IConfiguration configuration)
    {
        Initialized = true;
    }
}
