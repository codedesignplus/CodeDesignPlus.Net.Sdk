using System.Reflection;

namespace CodeDesignPlus.Net.Core.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Assembly"/> class.
/// </summary>
internal static class AssemblyExtensions
{
    /// <summary>
    /// Safely gets the types defined in the assembly, handling ReflectionTypeLoadException.
    /// </summary>
    /// <param name="assembly">The assembly to get types from.</param>
    /// <returns>An enumerable of types that were successfully loaded.</returns>
    /// <remarks>
    /// This method handles ReflectionTypeLoadException which can occur when an assembly
    /// references types from another assembly that is not available at runtime
    /// (e.g., Microsoft.CodeAnalysis which is typically only needed at build time).
    /// </remarks>
    public static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Return only the types that were successfully loaded
            return ex.Types.Where(t => t != null)!;
        }
        catch (Exception)
        {
            // For any other exception, return an empty collection to prevent crashes
            return Enumerable.Empty<Type>();
        }
    }

    /// <summary>
    /// Safely gets types from multiple assemblies, handling exceptions for each assembly.
    /// </summary>
    /// <param name="assemblies">The assemblies to get types from.</param>
    /// <returns>An enumerable of all types that were successfully loaded from all assemblies.</returns>
    public static IEnumerable<Type> SelectManyTypesSafely(this IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        return assemblies.SelectMany(assembly => assembly.GetTypesSafely());
    }
}
