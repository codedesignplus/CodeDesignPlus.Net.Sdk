using System;
using System.Collections.Immutable;
using CodeDesignPlus.Net.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDesignPlus.Net.Generator.Core
{
    internal class TargetTypeTracker : ISyntaxContextReceiver
    {
        public IImmutableList<TypeDeclarationSyntax> TypesNeedingDtoGening = ImmutableList.Create<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is TypeDeclarationSyntax cdecl && cdecl.IsDecoratedWithAttribute("dtogenerate"))
                TypesNeedingDtoGening = TypesNeedingDtoGening.Add(context.Node as TypeDeclarationSyntax);
        }
    }
}
