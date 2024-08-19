using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDesignPlus.Net.Generator.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    public static class TypeDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Determines whether the specified type declaration is decorated with the given attribute.
        /// </summary>
        /// <param name="typeDeclaration">The type declaration syntax.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns><c>true</c> if the type declaration is decorated with the attribute; otherwise, <c>false</c>.</returns>
        internal static bool IsDecoratedWithAttribute(this TypeDeclarationSyntax typeDeclaration, string attributeName) =>
            typeDeclaration.AttributeLists
                .SelectMany(x => x.Attributes)
                .Any(x => x.Name.ToString().ToLower() == attributeName.ToLower());
    }
}