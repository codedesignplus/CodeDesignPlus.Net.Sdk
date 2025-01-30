using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.FluentValidation;

/// <summary>
/// Provides extension methods for FluentValidation.
/// </summary>
public static class FluentExtensions
{
    /// <summary>
    /// Adds FluentValidation validators from the current AppDomain's assemblies to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the validators to.</param>
    /// <param name="lifetime">The lifetime of the validators.</param>
    /// <returns>The IServiceCollection with the added validators.</returns>
    /// <exception cref="ArgumentNullException">Thrown if no validator is found in the current AppDomain's assemblies.</exception>
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
