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
            //Debugger.Launch();

            // 1. Obtener todos los símbolos de clase decorados con el atributo DtoGenerator
            IncrementalValuesProvider<INamedTypeSymbol> commands = context.SyntaxProvider
             .CreateSyntaxProvider(
                 predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0,
                 transform: static (ctx, _) => GetClassWithDtoGeneratorAttribute(ctx)
             )
             .Where(static m => m is not null)
             .Select(static (m, _) => m!);

            // 2. Generar los DTOs para cada clase encontrada
            context.RegisterSourceOutput(commands, static (context, command) => GenerateDto(context, command));
        }

        /// <summary>
        /// Obtains the class if it is decorated with the DtoGenerator Attribute
        /// </summary>
        /// <param name="ctx">The GeneratorSyntaxContext to use</param>
        /// <returns>The INamedTypeSymbol if the class is decorated with the DtoGenerator attribute, null otherwise</returns>
        private static INamedTypeSymbol GetClassWithDtoGeneratorAttribute(GeneratorSyntaxContext ctx)
        {
            if (ctx.Node is not ClassDeclarationSyntax classDeclarationSyntax)
                return null;

            if (ctx.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                return null;

            if (!namedTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "DtoGenerator"))
                return null;

            return namedTypeSymbol;
        }


        private static void GenerateDto(SourceProductionContext context, INamedTypeSymbol command)
        {
            var codeBuilder = new StringBuilder();
            var dtoName = command.ContainingType != null ? $"{command.ContainingType.Name}Dto" : Regex.Replace(command.Name, PATTERN, "Dto", RegexOptions.None, System.TimeSpan.FromSeconds(1));
            codeBuilder.AppendLine($"namespace CodeDesignPlus.Microservice.Api.Dtos");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine($"public class {dtoName}");
            codeBuilder.AppendLine("{");
            
            //Log para generar la clase dto
             context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor("CDP005", "Generating DTO", $"Generating DTO {dtoName}", "CodeDesignPlus.Generator", DiagnosticSeverity.Info, isEnabledByDefault: true),
                command.Locations.FirstOrDefault()));

            AddProperties(codeBuilder, command, context);

            codeBuilder.AppendLine("}");
            codeBuilder.AppendLine("}");

            context.AddSource($"{dtoName}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));

            //Log para indicar que se finalizo la generacion
             context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor("CDP006", "Generated DTO", $"Generated DTO {dtoName}", "CodeDesignPlus.Generator", DiagnosticSeverity.Info, isEnabledByDefault: true),
                command.Locations.FirstOrDefault()));
        }

        private static void AddProperties(StringBuilder codeBuilder, INamedTypeSymbol command, SourceProductionContext context)
        {
             //Log para agregar las propiedades
             context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("CDP007", "Adding Properties", $"Adding properties for {command.Name}", "CodeDesignPlus.Generator", DiagnosticSeverity.Info, isEnabledByDefault: true),
                    command.Locations.FirstOrDefault()));

            var properties = command.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(prop => prop.DeclaredAccessibility == Accessibility.Public && !prop.IsStatic && !prop.IsReadOnly && prop.Name != "EqualityContract")
                .Select(prop => $"\t\t public {prop.Type.ToDisplayString()} {prop.Name} {{ get; set; }}");

            foreach (var property in properties)
            {
                codeBuilder.AppendLine(property);

                //Log individual de la propiedad agregada
                  context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("CDP008", "Added Property", $"Added property {property}", "CodeDesignPlus.Generator", DiagnosticSeverity.Info, isEnabledByDefault: true),
                     command.Locations.FirstOrDefault()));
            }
        }
    }
}