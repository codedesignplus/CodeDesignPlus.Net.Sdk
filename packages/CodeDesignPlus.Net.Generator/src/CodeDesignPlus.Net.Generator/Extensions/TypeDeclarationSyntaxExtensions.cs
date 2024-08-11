using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDesignPlus.Net.Generator.Extensions
{
    public static class TypeDeclarationSyntaxExtensions
    {
        internal static bool IsDecoratedWithAttribute(
            this TypeDeclarationSyntax cdecl, string attributeName) =>
            cdecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .Any(x => x.Name.ToString().ToLower() == attributeName);
    }
}
