using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Core.Sample.Resources.Startup;

public class StartupCustom : IStartup
{
    public void Initialize(IServiceCollection services, IConfiguration configuration)
    {
        throw new NotImplementedException();
    }
}
