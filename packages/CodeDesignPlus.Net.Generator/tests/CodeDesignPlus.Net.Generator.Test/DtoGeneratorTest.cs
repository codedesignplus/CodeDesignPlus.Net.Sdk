using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeDesignPlus.Net.Generator.Test;

public class DtoGeneratorTest
{
    [Theory]
    [InlineData("Class")]
    [InlineData("Record")]
    public void Execute_CommandExist_CreateDto(string commandType)
    {
        // Arrange
        var sourceExpected = "namespace CodeDesignPlus.Microservice.Api.Dtos\r\n{\r\npublic class CreateUserDto\r\n{\r\n\t\t public string? Name { get; set; }\r\n\t\t public int? Age { get; set; }\r\n}\r\n}\r\n";

        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            sourceExpected = sourceExpected.Replace("\r\n", "\n");

        var sourceRecord = """
        using System;
        
        namespace CodeDesignPlus.Microservice.Application.Commands
        {
           [DtoGeneratorAttribute]
           public record CreateUserCommand(string? Name, int? Age);
        }
        """;

        var sourceClass = """
        using System;
        
        namespace CodeDesignPlus.Microservice.Application.Commands
        {
            [DtoGeneratorAttribute]
            public class CreateUserCommand
            {
                public string? Name { get; set; }
                public int? Age { get; set; }
            }
        }
        """;

        var source = commandType == "Record" ? sourceRecord : sourceClass;

        var syntaxTree = SyntaxFactory.ParseSyntaxTree(source);

        var compilation = CSharpCompilation.Create("CodeDesignPlus.Net.Dummy",
            syntaxTrees: [syntaxTree],
             references: [ MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
              MetadataReference.CreateFromFile(typeof(CodeDesignPlus.Net.Generator.DtoGenerator).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var generator = new DtoGenerator();

        // Act
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.Empty(diagnostics);
        var runResult = driver.GetRunResult();
        Assert.Empty(runResult.Diagnostics);
        Assert.Single(runResult.Results);
        Assert.NotEmpty(runResult.GeneratedTrees); // Verifica que al menos un árbol fue generado
        Assert.Single(runResult.Results[0].GeneratedSources);

        var generatedSource = runResult.Results[0].GeneratedSources[0];

        Assert.Equal(sourceExpected.Trim(), generatedSource.SourceText.ToString().Trim());
    }


    /// <summary>
    /// [NotMoney] del comando llega al DTO: el DTO es lo que describe el Swagger (regla 74).
    /// </summary>
    [Theory]
    [InlineData("Record")]
    [InlineData("Class")]
    public void Execute_CommandWithNotMoney_CopiesTheAttributeToTheDto(string commandType)
    {
        const string attribute = """
        namespace CodeDesignPlus.Net.Core.Abstractions.Attributes
        {
            [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
            public sealed class NotMoneyAttribute : System.Attribute { }
        }
        """;

        var command = commandType == "Record"
            ? """
              namespace CodeDesignPlus.Microservice.Application.Commands
              {
                  [DtoGeneratorAttribute]
                  public record CreateRangeCommand([property: CodeDesignPlus.Net.Core.Abstractions.Attributes.NotMoney] long From, long Amount);
              }
              """
            : """
              namespace CodeDesignPlus.Microservice.Application.Commands
              {
                  [DtoGeneratorAttribute]
                  public class CreateRangeCommand
                  {
                      [CodeDesignPlus.Net.Core.Abstractions.Attributes.NotMoney]
                      public long From { get; set; }

                      public long Amount { get; set; }
                  }
              }
              """;

        var compilation = CSharpCompilation.Create("CodeDesignPlus.Net.Dummy",
            syntaxTrees: [SyntaxFactory.ParseSyntaxTree(attribute), SyntaxFactory.ParseSyntaxTree(command)],
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CodeDesignPlus.Net.Generator.DtoGenerator).Assembly.Location)],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new DtoGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);

        Assert.Empty(diagnostics);

        var generated = Assert.Single(driver.GetRunResult().Results[0].GeneratedSources).SourceText.ToString();
        var lines = generated.Split('\n').Select(x => x.Trim()).ToList();
        var from = lines.FindIndex(x => x.Contains("long From"));
        var amount = lines.FindIndex(x => x.Contains("long Amount"));

        Assert.Equal("[global::CodeDesignPlus.Net.Core.Abstractions.Attributes.NotMoneyAttribute]", lines[from - 1]);
        Assert.DoesNotContain("NotMoney", lines[amount - 1]);

        // El DTO generado compila con el atributo (los errores del comando de prueba, que no resuelve DtoGenerator, no cuentan).
        var dtoTree = Assert.Single(driver.GetRunResult().GeneratedTrees);

        Assert.DoesNotContain(output.GetDiagnostics(), x => x.Severity == DiagnosticSeverity.Error && x.Location.SourceTree == dtoTree);
    }

    [Fact]
    public void Execute_AssemblyApplicationNotExist_DoesNotGenerateDto()
    {
        // Arrange
        var source = @"
        using System;

        namespace CodeDesignPlus.Microservice.Application.Commands
        {
            public class CreateUserCommand
            {
                public string UserName { get; set; }
                public string Email { get; set; }
            }
        }
        ";
        var syntaxTree = SyntaxFactory.ParseSyntaxTree(source);


        var compilation = CSharpCompilation.Create("CodeDesignPlus.Net.Dummy",
            syntaxTrees: [syntaxTree],
            references: [],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var generator = new DtoGenerator();

        // Act
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        // Ensure that no diagnostics were created by the generators
        Assert.Empty(diagnostics);
        // Ensure that only the original syntax tree is present
        Assert.Single(outputCompilation.SyntaxTrees);

        // Obtain the generator run result
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        // The runResult should have no generated trees
        Assert.Empty(runResult.GeneratedTrees);
        Assert.Empty(runResult.Diagnostics);

        // Access individual results on a by-generator basis
        GeneratorRunResult generatorResult = runResult.Results[0];
        //Assert.Equal(dtoGenerator, generatorResult.Generator);
        Assert.Empty(generatorResult.Diagnostics);
        Assert.Empty(generatorResult.GeneratedSources);
        Assert.Null(generatorResult.Exception);
    }
    
}