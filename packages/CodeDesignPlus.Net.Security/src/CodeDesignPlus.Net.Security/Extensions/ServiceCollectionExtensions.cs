using CodeDesignPlus.Net.Security.Middlewares;
using CodeDesignPlus.Net.Security.MIddlewares;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeDesignPlus.Net.Security.Extensions;

/// <summary>
/// Provides extension methods for setting up security services in an <see cref="IServiceCollection"/> and configuring authentication in an <see cref="IApplicationBuilder"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds security services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration to bind the security options.</param>
    /// <param name="options">Optional action to configure the <see cref="JwtBearerOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="SecurityException">Thrown if the security configuration section does not exist.</exception>
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
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var securityOptions = section.Get<SecurityOptions>();

        services
            .TryAddScoped<IUserContext, UserContext>();

        services
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(configuration, options);

        if (securityOptions.ValidateRbac)
        {
            services.AddGrpcClient<gRpc.Rbac.RbacClient>(o =>
            {
                o.Address = securityOptions.ServerRbac;
            });

            services.TryAddScoped<IRbac, Rbac>();
            services.AddHostedService<RefreshRbacBackgroundService>();
        }

        if (securityOptions.EnableTenantContext)
        {
            services.TryAddScoped<ITenant, Tenant>();
        }

        return services;
    }

    /// <summary>
    /// Configures the application to use authentication and authorization.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        var options = app.ApplicationServices.GetRequiredService<IOptions<SecurityOptions>>().Value;

        if (options.EnableTenantContext && options.ValidateLicense)
            app.UseMiddleware<LicenseMiddleware>();

        if (options.ValidateRbac)
            app.UseMiddleware<RbacMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds JWT Bearer authentication to the specified <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="authenticationBuilder">The <see cref="AuthenticationBuilder"/> to add the authentication to.</param>
    /// <param name="configuration">The configuration to bind the security options.</param>
    /// <param name="options">Optional action to configure the <see cref="JwtBearerOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> so that additional calls can be chained.</returns>
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
                        ValidateLifetime = securityOptions.ValidateLifetime,
                        ValidateAudience = securityOptions.ValidateAudience,
                        ValidAudiences = securityOptions.ValidAudiences,
                        SignatureValidator = (token, _) => new JsonWebToken(token)
                    };

                    if (securityOptions.ValidIssuers.Count == 0)
                        x.TokenValidationParameters.ValidIssuer = securityOptions.ValidIssuer;
                    else
                        x.TokenValidationParameters.ValidIssuers = securityOptions.ValidIssuers;


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
                        OnAuthenticationFailed = AuthenticationFailed,
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var tenantFromQuery = context.Request.Query["x-tenant"];

                            var path = context.HttpContext.Request.Path;

                            if (path.Value.Contains("/hubs/") || path.Value.Contains("/hub/"))
                            {
                                if (!string.IsNullOrEmpty(accessToken))

                                    context.Token = accessToken;

                                if (!string.IsNullOrEmpty(tenantFromQuery))
                                    context.Request.Headers["X-Tenant"] = tenantFromQuery.ToString();

                            }
                            return Task.CompletedTask;
                        }
                    };
                }
            );

        return authenticationBuilder;
    }

    /// <summary>
    /// Handles authentication failures by writing appropriate responses.
    /// </summary>
    /// <param name="context">The authentication failed context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal static async Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        var exception = context.Exception;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.HandleTokenException(exception);
    }
}