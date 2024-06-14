using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDesignPlus.Net.Generator
{
    internal class TargetTypeTracker : ISyntaxContextReceiver
    {
        public IImmutableList<TypeDeclarationSyntax> TypesNeedingDtoGening = ImmutableList.Create<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {

            if (context.Node is TypeDeclarationSyntax cdecl)
                if (cdecl.IsDecoratedWithAttribute("dtogenerate"))
                    TypesNeedingDtoGening = TypesNeedingDtoGening.Add(context.Node as TypeDeclarationSyntax);
        }
    }

    internal static class SourceGenExtns
    {
        internal static bool IsDecoratedWithAttribute(
            this TypeDeclarationSyntax cdecl, string attributeName) =>
            cdecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .Any(x => x.Name.ToString().ToLower() == attributeName);
    }
}
