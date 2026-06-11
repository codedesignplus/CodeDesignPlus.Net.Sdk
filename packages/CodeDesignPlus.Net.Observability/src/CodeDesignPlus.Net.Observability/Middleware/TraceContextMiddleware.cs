using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.Observability.Middleware;

/// <summary>
/// Middleware that adds the current trace ID to response headers.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TraceContextMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public class TraceContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var activity = Activity.Current;
            if (activity != null)
            {
                context.Response.Headers["X-Trace-Id"] = activity.TraceId.ToString();
                context.Response.Headers["X-Span-Id"] = activity.SpanId.ToString();
            }
            return Task.CompletedTask;
        });

        await next(context);
    }
}
