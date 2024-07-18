
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeDesignPlus.Net.Generator
{

    [Generator]
    public class DtoGenerator : ISourceGenerator
    {
        private const string PATTERN = @"(Comman?d?s?)$";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TargetTypeTracker());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Debugger.Launch();

            // Obtener todos los árboles de sintaxis del proyecto principal
            var syntaxTrees = context.Compilation.SyntaxTrees;

            // Obtener los ensamblados referenciados
            var referencedAssemblies = context.Compilation.ReferencedAssemblyNames;

            // Encontrar la referencia al ensamblado de la capa de aplicación
            var applicationReference = referencedAssemblies.FirstOrDefault(x => x.Name.Contains("Application"));

            if (applicationReference != null)
            {
                // Obtener el ensamblado de la capa de aplicación
                var assembly = context.Compilation.References
                    .Select(r => context.Compilation.GetAssemblyOrModuleSymbol(r))
                    .OfType<IAssemblySymbol>()
                    .FirstOrDefault(a => a?.Name == applicationReference.Name);

                if (assembly != null)
                {
                    // Verificar y registrar los tipos obtenidos
                    var allTypes = assembly.GlobalNamespace.GetNamespaceTypes().ToList();
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("INFO1", "Info", $"Total types in Application assembly: {allTypes.Count}", "Info", DiagnosticSeverity.Info, true), Location.None));

                    // Encontrar todas las clases con el atributo GenerateDto en el ensamblado de la capa de aplicación
                    var commands = allTypes
                        .Where(t => t.GetAttributes().Any(attr => attr.AttributeClass?.Name == "DtoGeneratorAttribute"))
                        .ToList();

                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("INFO2", "Info", $"Total classes with GenerateDto attribute found: {commands.Count}", "Info", DiagnosticSeverity.Info, true), Location.None));

                    GenerateDtos(context, commands);
                }
            }
        }

        private void GenerateDtos(GeneratorExecutionContext context, List<INamedTypeSymbol> commands)
        {
            var codeBuilder = new StringBuilder();

            foreach (var command in commands)
            {
                var dtoName = command.ContainingType != null ? $"{command.ContainingType.Name}Dto" : Regex.Replace(command.Name, PATTERN, "Dto");
                var name = command.ContainingType != null ? command.ContainingType.Name : command.Name;

                var properties = command.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Select(prop => $"public {prop.Type.ToDisplayString()} {prop.Name} {{ get; set; }}")
                    .ToArray();

                codeBuilder.AppendLine($"// Generate from {name}");
                codeBuilder.AppendLine("using System;");
                codeBuilder.AppendLine("using System;");
                codeBuilder.AppendLine("using System.Collections.Generic;");
                codeBuilder.AppendLine("using System.Linq;");
                codeBuilder.AppendLine("");


                // Add target namespace
                codeBuilder.AppendLine($"namespace CodeDesignPlus.Microservice.Api.Dtos");
                codeBuilder.AppendLine("{");

                // Start class
                codeBuilder.AppendLine($"\tpublic class {dtoName}");
                codeBuilder.AppendLine("\t{");

                // get all the properties defined in this class
                foreach (var property in command.GetMembers().OfType<IPropertySymbol>().Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic && !x.IsReadOnly && x.Name != "EqualityContract"))
                {
                    codeBuilder.AppendLine($"\t\tpublic {property.Type.ToDisplayString()} {property.Name} {{ get; set; }}");
                }

                // Add closing braces
                codeBuilder.AppendLine("\t}");
                codeBuilder.AppendLine("}");

                // add the code for this DTO class to the context so it can be added to the build
                context.AddSource($"{dtoName}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));
                codeBuilder.Clear();
            }
        }
    }

}