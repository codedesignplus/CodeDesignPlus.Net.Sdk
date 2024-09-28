using System.Collections.Immutable;
using CodeDesignPlus.Net.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDesignPlus.Net.Generator.Core
{
    /// <summary>
    /// Tracks types that need DTO generation.
    /// </summary>
    internal class TargetTypeTracker : ISyntaxContextReceiver
    {
        /// <summary>
        /// Gets the list of types that need DTO generation.
        /// </summary>
        public IImmutableList<TypeDeclarationSyntax> TypesNeedingDtoGening { get; private set; } = ImmutableList.Create<TypeDeclarationSyntax>();

        /// <summary>
        /// Called when a syntax node is visited.
        /// </summary>
        /// <param name="context">The generator syntax context.</param>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is TypeDeclarationSyntax cdecl && cdecl.IsDecoratedWithAttribute("dtogenerate"))
            {
                TypesNeedingDtoGening = TypesNeedingDtoGening.Add(cdecl);
            }
        }
    }
}