﻿using CodeDesignPlus.Net.Generator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeDesignPlus.Net.Generator
{
    /// <summary>
    /// Source generator for creating DTO classes.
    /// </summary>
    /// <remarks>https://andrewlock.net/series/creating-a-source-generator/</remarks>
    [Generator(LanguageNames.CSharp)]
    public class DtoGenerator : IIncrementalGenerator
    {
        private const string PATTERN = @"(Comman?d?s?)$";

        /// <summary>
        /// Initializes the generator.
        /// </summary>
        /// <param name="context">The generator initialization context.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<INamedTypeSymbol> commands = context.SyntaxProvider
             .CreateSyntaxProvider(
                 predicate: static (syntaxNode, _) => FilterItems(syntaxNode),
                 transform: static (ctx, _) => GetClassWithDtoGeneratorAttribute(ctx)
             )
             .Where(static m => m is not null)
             .Select(static (m, _) => m!);

            context.RegisterSourceOutput(commands, static (context, command) => GenerateDto(context, command));
        }

        /// <summary>
        /// Filter the items that will be processed by the generator
        /// </summary>
        /// <param name="syntaxNode">The syntax node to filter</param>
        /// <returns>True if the syntax node is a class or record decorated with the DtoGenerator attribute, false otherwise</returns>
        public static bool FilterItems(SyntaxNode syntaxNode)
        {
            if(syntaxNode is RecordDeclarationSyntax recordDeclaration && recordDeclaration.AttributeLists.Count > 0)
                return true;

            if (syntaxNode is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Obtains the class if it is decorated with the DtoGenerator Attribute
        /// </summary>
        /// <param name="ctx">The GeneratorSyntaxContext to use</param>
        /// <returns>The INamedTypeSymbol if the class is decorated with the DtoGenerator attribute, null otherwise</returns>
        private static INamedTypeSymbol GetClassWithDtoGeneratorAttribute(GeneratorSyntaxContext ctx)
        {
            if (ctx.Node is ClassDeclarationSyntax classDeclarationSyntax) {
                if (ctx.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                    return null;

                if (!namedTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(DtoGeneratorAttribute)))
                    return null;

                return namedTypeSymbol;
            }

            if (ctx.Node is RecordDeclarationSyntax recordDeclarationSyntax)
            {
                if (ctx.SemanticModel.GetDeclaredSymbol(recordDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                    return null;

                if (!namedTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(DtoGeneratorAttribute)))
                    return null;

                return namedTypeSymbol;
            }

            return null;
        }

        /// <summary>
        /// Generates the DTO class from the command class
        /// </summary>
        /// <param name="context">Context for the source generation</param>
        /// <param name="command">The command class to generate the DTO</param>
        private static void GenerateDto(SourceProductionContext context, INamedTypeSymbol command)
        {
            var codeBuilder = new StringBuilder();
            var dtoName = command.ContainingType != null ? $"{command.ContainingType.Name}Dto" : Regex.Replace(command.Name, PATTERN, "Dto", RegexOptions.None, System.TimeSpan.FromSeconds(1));
            codeBuilder.AppendLine($"namespace CodeDesignPlus.Microservice.Api.Dtos");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine($"public class {dtoName}");
            codeBuilder.AppendLine("{");
            
            AddProperties(codeBuilder, command);

            codeBuilder.AppendLine("}");
            codeBuilder.AppendLine("}");

            context.AddSource($"{dtoName}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));
        }

        /// <summary>
        /// Add the properties to the DTO class
        /// </summary>
        /// <param name="codeBuilder">StringBuilder to add the properties</param>
        /// <param name="command">The command class to generate the DTO</param>
        private static void AddProperties(StringBuilder codeBuilder, INamedTypeSymbol command)
        {
            var properties = command.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(prop => prop.DeclaredAccessibility == Accessibility.Public && !prop.IsStatic && !prop.IsReadOnly && prop.Name != "EqualityContract")
                .Select(prop => $"\t\t public {prop.Type.ToDisplayString()} {prop.Name} {{ get; set; }}");

            foreach (var property in properties)
            {
                codeBuilder.AppendLine(property);
            }
        }
    }
}