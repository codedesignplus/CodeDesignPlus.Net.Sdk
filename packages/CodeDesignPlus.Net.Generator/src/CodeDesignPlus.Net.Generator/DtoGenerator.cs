using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CodeDesignPlus.Net.Generator.Extensions;
using CodeDesignPlus.Net.Generator.Core;

namespace CodeDesignPlus.Net.Generator
{
    /// <summary>
    /// Source generator for creating DTO classes.
    /// </summary>
    [Generator]
    public class DtoGenerator : ISourceGenerator
    {
        private const string PATTERN = @"(Comman?d?s?)$";

        /// <summary>
        /// Initializes the generator.
        /// </summary>
        /// <param name="context">The generator initialization context.</param>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TargetTypeTracker());
        }

        /// <summary>
        /// Executes the generator.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        public void Execute(GeneratorExecutionContext context)
        {
            var applicationReference = context.Compilation.ReferencedAssemblyNames.FirstOrDefault(x => x.Name.Contains("Application"));

            if (applicationReference is null)
                return;

            var assembly = context.Compilation.References
                .Select(r => context.Compilation.GetAssemblyOrModuleSymbol(r))
                .OfType<IAssemblySymbol>()
                .First(x => x.Name == applicationReference.Name);

            var allTypes = assembly.GlobalNamespace.GetNamespaceTypes().ToList();

            var commands = allTypes
                .Where(t => t.GetAttributes().Any(attr => attr.AttributeClass.Name == "DtoGeneratorAttribute"))
                .ToList();

            GenerateDtos(context, commands);
        }

        /// <summary>
        /// Generates DTO classes for the specified commands.
        /// </summary>
        /// <param name="context">The generator execution context.</param>
        /// <param name="commands">The list of commands to generate DTOs for.</param>
        private void GenerateDtos(GeneratorExecutionContext context, List<INamedTypeSymbol> commands)
        {
            var codeBuilder = new StringBuilder();

            foreach (var command in commands)
            {
                var dtoName = command.ContainingType != null ? $"{command.ContainingType.Name}Dto" : Regex.Replace(command.Name, PATTERN, "Dto", RegexOptions.None, System.TimeSpan.FromSeconds(1));

                codeBuilder.AppendLine($"namespace CodeDesignPlus.Microservice.Api.Dtos");
                codeBuilder.AppendLine("{");
                codeBuilder.AppendLine($"public class {dtoName}");
                codeBuilder.AppendLine("{");

                AddProperties(codeBuilder, command);

                codeBuilder.AppendLine("}");
                codeBuilder.AppendLine("}");

                context.AddSource($"{dtoName}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));
                codeBuilder.Clear();
            }
        }

        /// <summary>
        /// Adds properties to the DTO class.
        /// </summary>
        /// <param name="codeBuilder">The code builder.</param>
        /// <param name="command">The command symbol.</param>
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