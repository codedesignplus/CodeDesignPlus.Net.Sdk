using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace CodeDesignPlus.Net.Generator.Extensions
{
    /// <summary>
    /// Extension methods for working with Roslyn symbols.
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Gets all types within a namespace, including nested types.
        /// </summary>
        /// <param name="namespace">The namespace symbol.</param>
        /// <returns>An enumerable of named type symbols.</returns>
        public static IEnumerable<INamedTypeSymbol> GetNamespaceTypes(this INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
            {
                yield return type;

                foreach (var nestedType in type.GetAllNestedTypes())
                {
                    yield return nestedType;
                }
            }

            foreach (var subNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in subNamespace.GetNamespaceTypes())
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Gets all nested types within a type.
        /// </summary>
        /// <param name="type">The named type symbol.</param>
        /// <returns>An enumerable of named type symbols.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllNestedTypes(this INamedTypeSymbol type)
        {
            foreach (var nestedType in type.GetTypeMembers())
            {
                yield return nestedType;
            }
        }
    }
}