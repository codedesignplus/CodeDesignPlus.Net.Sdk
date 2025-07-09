using CodeDesignPlus.Net.Core.Abstractions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.Application;

/// <summary>
/// Provides extension methods for configuring the application pipeline.
/// This class is used to set up the path base for the application based on the CoreOptions
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Configures the application to use a path base if specified in CoreOptions.
    /// This method checks the CoreOptions for a specified PathBase and applies it to the application
    /// </summary>
    /// <param name="app">An instance of IApplicationBuilder to configure the application pipeline.</param>
    public static void UsePath(this IApplicationBuilder app)
    {
        var value = app.ApplicationServices.GetRequiredService<IOptions<CoreOptions>>().Value;

        if (!string.IsNullOrEmpty(value.PathBase))
        {
            app.UsePathBase(value.PathBase);
        }
    }
}