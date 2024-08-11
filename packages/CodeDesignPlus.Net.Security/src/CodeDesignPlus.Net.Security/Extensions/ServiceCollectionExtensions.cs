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
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

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
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var securityOptions = configuration.GetSection(SecurityOptions.Section).Get<SecurityOptions>();

        var certificate = securityOptions.GetCertificate();

        authenticationBuilder
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                x =>
                {
                    if (!string.IsNullOrEmpty(securityOptions.Authority))
                        x.Authority = securityOptions.Authority;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = securityOptions.ValidateIssuer,
                        ValidateLifetime = securityOptions.ValidateLifetime,
                        ValidIssuer = securityOptions.ValidIssuer,
                        ValidateAudience = securityOptions.ValidateAudience,
                        ValidAudiences = securityOptions.ValidAudiences,
                        SignatureValidator = (token, _) => new JsonWebToken(token)
                    };

                    if (certificate != null)
                    {
                        x.TokenValidationParameters.ValidateIssuerSigningKey = false;
                        x.TokenValidationParameters.RequireSignedTokens = true;
                        x.TokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
                    }

                    x.IncludeErrorDetails = securityOptions.IncludeErrorDetails;
                    x.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;

                    options?.Invoke(x);

                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = AuthenticationFailed
                    };
                }
            );

        return authenticationBuilder;
    }

    internal static async Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        var exception = context.Exception;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.HandleTokenException(exception);
    }
}
