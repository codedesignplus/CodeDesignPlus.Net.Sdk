
using CodeDesignPlus.Net.Exceptions.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.Exceptions.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddlware>();
    }
}