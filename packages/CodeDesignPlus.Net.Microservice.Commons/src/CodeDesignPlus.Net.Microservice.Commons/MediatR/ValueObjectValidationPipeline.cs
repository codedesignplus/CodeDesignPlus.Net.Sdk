using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Commons.MediatR;

/// <summary>
/// Validates every value object a request carries before its handler runs.
/// </summary>
/// <remarks>
/// A value object's constructor only hydrates, because Mongo, the cache and the deserialization of events rebuild it
/// through it and a read must never fail (Kappali pendings/737). The same constructor deserializes the body of a request,
/// so the invariants that used to reject an invalid value there are checked here, with the same errors, before the
/// command reaches the aggregate.
/// </remarks>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValueObjectValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    /// <summary>
    /// Calls <see cref="IValidatableValueObject.EnsureValid"/> on each value object of the request and continues the pipeline.
    /// </summary>
    /// <param name="request">Client request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Server response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValueObjectWalker.EnsureValid(request);

        return await next(cancellationToken);
    }
}

/// <summary>
/// Walks an object graph and validates the value objects it finds.
/// </summary>
internal static class ValueObjectWalker
{
    /// <summary>
    /// How deep the walk goes. A request is a command or a query with its DTOs, never a deep graph; the limit only guards
    /// against an unexpected one.
    /// </summary>
    private const int MaxDepth = 8;

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> properties = new();

    private static readonly string[] frameworkNamespaces = ["System", "Microsoft", "NodaTime", "MongoDB", "Newtonsoft", "Google", "Grpc"];

    /// <summary>
    /// Calls <see cref="IValidatableValueObject.EnsureValid"/> on every value object reachable from <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The root of the graph, usually a request.</param>
    public static void EnsureValid(object? value)
    {
        Visit(value, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
    }

    private static void Visit(object? value, int depth, HashSet<object> visited)
    {
        if (value is null || depth > MaxDepth)
        {
            return;
        }

        // A value object validates the value objects it contains.
        if (value is IValidatableValueObject valueObject)
        {
            valueObject.EnsureValid();

            return;
        }

        var type = value.GetType();

        if (type.IsValueType || value is string || IsArrayOfValues(type) || !visited.Add(value))
        {
            return;
        }

        if (value is IDictionary dictionary)
        {
            foreach (var item in dictionary.Values)
            {
                Visit(item, depth + 1, visited);
            }

            return;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Visit(item, depth + 1, visited);
            }

            return;
        }

        if (IsFrameworkType(type))
        {
            return;
        }

        foreach (var property in properties.GetOrAdd(type, Readable))
        {
            Visit(property.GetValue(value), depth + 1, visited);
        }
    }

    private static bool IsArrayOfValues(Type type) => type.IsArray && type.GetElementType()!.IsValueType;

    private static bool IsFrameworkType(Type type)
    {
        var name = type.Namespace ?? string.Empty;

        return frameworkNamespaces.Any(x => name == x || name.StartsWith(x + ".", StringComparison.Ordinal));
    }

    private static PropertyInfo[] Readable(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanRead && x.GetIndexParameters().Length == 0 && x.GetMethod!.IsPublic)
            .ToArray();
    }
}
