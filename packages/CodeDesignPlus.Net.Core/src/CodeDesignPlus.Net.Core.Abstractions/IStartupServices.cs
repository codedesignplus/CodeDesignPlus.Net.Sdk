using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Core.Abstractions
{
    /// <summary>
    /// Specification of the contract for the management of custom services in the CodeDesignPlus SDK.
    /// </summary>
    public interface IStartupServices
    {
        /// <summary>
        /// This method is invoked by the CodeDesignPlus SDK at the start of the application to register custom services.
        /// </summary>
        /// <param name="services">Provides access to the .net core dependency container.</param>
        /// <param name="configuration">Provides access to the various configuration sources.</param>
        void Initialize(IServiceCollection services, IConfiguration configuration);
    }
}
