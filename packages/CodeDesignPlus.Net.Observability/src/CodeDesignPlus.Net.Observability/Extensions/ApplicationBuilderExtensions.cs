using Microsoft.AspNetCore.Builder;
using CodeDesignPlus.Net.Observability.Middleware;

namespace CodeDesignPlus.Net.Observability.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to configure observability middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the trace context middleware to the pipeline.
    /// This middleware adds X-Trace-Id and X-Span-Id headers to all responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTraceContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TraceContextMiddleware>();
    }
}
