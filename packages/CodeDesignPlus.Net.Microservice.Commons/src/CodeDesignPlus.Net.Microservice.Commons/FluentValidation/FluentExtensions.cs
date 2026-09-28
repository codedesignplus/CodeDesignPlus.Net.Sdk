using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.FluentValidation;

/// <summary>
/// Provides extension methods for FluentValidation.
/// </summary>
public static class FluentExtensions
{
    /// <summary>
    /// Adds the FluentValidation validators declared in the assembly that contains <typeparamref name="TStartup"/>.
    /// </summary>
    /// <remarks>
    /// The assembly is explicit, so the result does not depend on which assemblies the runtime has loaded when the
    /// method runs. An assembly without validators is valid and registers none.
    /// </remarks>
    /// <typeparam name="TStartup">A type from the assembly that declares the validators, usually the Application startup.</typeparam>
    /// <param name="services">The IServiceCollection to add the validators to.</param>
    /// <param name="lifetime">The lifetime of the validators.</param>
    /// <returns>The IServiceCollection with the added validators.</returns>
    public static IServiceCollection AddFluentValidation<TStartup>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddValidatorsFromAssemblyContaining<TStartup>(lifetime);

        return services;
    }

    /// <summary>
    /// Adds FluentValidation validators from the current AppDomain's assemblies to the specified IServiceCollection.
    /// </summary>
    /// <remarks>
    /// Only the assemblies already loaded are inspected, and the first validator found decides which assembly is
    /// registered. With ReadyToRun the FluentValidation assembly may not be loaded yet, so a service without its own
    /// validators fails to start.
    /// </remarks>
    /// <param name="services">The IServiceCollection to add the validators to.</param>
    /// <param name="lifetime">The lifetime of the validators.</param>
    /// <returns>The IServiceCollection with the added validators.</returns>
    /// <exception cref="ArgumentNullException">Thrown if no validator is found in the current AppDomain's assemblies.</exception>
    [Obsolete("The result depends on the assembly load order. Use AddFluentValidation<TStartup>() with the Application startup.")]
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var validator = AppDomain.CurrentDomain
                   .GetAssemblies()
                   .SelectMany(x => x.GetTypes())
                   .FirstOrDefault(type => type.BaseType?.IsGenericType == true && type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>));

        ArgumentNullException.ThrowIfNull(validator);

        services.AddValidatorsFromAssembly(validator.Assembly, lifetime);

        return services;
    }
}
