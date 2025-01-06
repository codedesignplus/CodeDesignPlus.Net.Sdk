using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using CodeDesignPlus.Net.Generator;

namespace CodeDesignPlus.Net.Generator.Test;

public class DtoGeneratorTest
{
    [Fact]
    public void Execute_CommandExist_CreateDto()
    {
        // Arrange
        var sourceExpected = "namespace CodeDesignPlus.Microservice.Api.Dtos\r\n{\r\npublic class CreateUserDto\r\n{\r\n\t\t public string? Name { get; set; }\r\n\t\t public int? Age { get; set; }\r\n}\r\n}\r\n";

        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            sourceExpected = sourceExpected.Replace("\r\n", "\n");

        var source = """
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