using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace CodeDesignPlus.Net.Generator
{

    public static class SymbolExtensions
    {
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

        public static IEnumerable<INamedTypeSymbol> GetAllNestedTypes(this INamedTypeSymbol type)
        {
            foreach (var nestedType in type.GetTypeMembers())
            {
                yield return nestedType;

                foreach (var childNestedType in nestedType.GetAllNestedTypes())
                {
                    yield return childNestedType;
                }
            }
        }

    }
}
