using CodeDesignPlus.Net.Core.Abstractions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.Application;

public static class ApplicationExtensions
{
    public static void UsePath(this IApplicationBuilder app)
    {
        var value = app.ApplicationServices.GetRequiredService<IOptions<CoreOptions>>().Value;
        
        if (!string.IsNullOrEmpty(value.PathBase))
        {
            app.UsePathBase(value.PathBase);
        }
    }    

}