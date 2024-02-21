using CodeDesignPlus.Net.Security.Abstractions.Options;
using CodeDesignPlus.Net.Security.Exceptions;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CodeDesignPlus.Net.Security.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add CodeDesignPlus.EFCore configuration options
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <param name="options">Configure the JwtBearerOptions</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions> options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(SecurityOptions.Section);

        if (!section.Exists())
            throw new SecurityException($"The section {SecurityOptions.Section} is required.");

        services
            .AddOptions<SecurityOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services
            .AddSingleton<IUserContext, UserContext>()
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(configuration, options);

        return services;
    }

    /// <summary>
    /// Add JwtBearer authentication
    /// </summary>
    /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <returns>The Microsoft.AspNetCore.Builder.IApplicationBuilder so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Add JwtBearer authentication
    /// </summary>
    /// <param name="authenticationBuilder">The Microsoft.AspNetCore.Authentication.AuthenticationBuilder to add the authentication to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <param name="options">Configure the JwtBearerOptions</param>
    /// <returns>The Microsoft.AspNetCore.Authentication.AuthenticationBuilder so that additional calls can be chained.</returns>
    public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder authenticationBuilder, IConfiguration configuration, Action<JwtBearerOptions> options = null)
    {
        var securityOptions = configuration.GetSection(SecurityOptions.Section).Get<SecurityOptions>();

        authenticationBuilder
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                x =>
                {
                    if (!string.IsNullOrEmpty(securityOptions.Authority))
                        x.Authority = securityOptions.Authority;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = securityOptions.RequireSignedTokens,
                        ValidateAudience = securityOptions.ValidateAudience,
                        ValidateIssuer = securityOptions.ValidateIssuer,
                        ValidateLifetime = securityOptions.ValidateLifetime,
                        ValidateIssuerSigningKey = securityOptions.ValidateIssuerSigningKey,
                        ValidIssuer = securityOptions.ValidIssuer,
                        ValidAudiences = securityOptions.ValidAudiences
                    };

                    if (securityOptions.Certificate != null)
                        x.TokenValidationParameters.IssuerSigningKey = new X509SecurityKey(securityOptions.Certificate);

                    x.IncludeErrorDetails = securityOptions.IncludeErrorDetails;
                    x.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;

                    options?.Invoke(x);
                }
            );

        return authenticationBuilder;
    }
}
