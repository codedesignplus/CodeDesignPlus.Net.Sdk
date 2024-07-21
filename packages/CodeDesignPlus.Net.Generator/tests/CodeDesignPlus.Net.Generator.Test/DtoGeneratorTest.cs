using Xunit;
using CodeDesignPlus.Net.Generator.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;


namespace CodeDesignPlus.Net.Generator.Test;

public class DtoGeneratorTest
{

    [Fact]
    public void Execute_CommandExist_CreateDto()
    {
        // Arrange     
        var sourceExpected = "namespace CodeDesignPlus.Microservice.Api.Dtos\r\n{\r\npublic class CreateUserDto\r\n{\r\n\t\t public string? Name { get; set; }\r\n\t\t public int? Age { get; set; }\r\n}\r\n}\r\n";

        var syntaxTree = SyntaxFactory.ParseSyntaxTree(@"
        using System;

        [dtogenerate]
        class MyClass {}

        class AnotherClass {}");

        var compilation = CSharpCompilation.Create("CodeDesignPlus.Net.Dummy",
              syntaxTrees: [syntaxTree],
              references: [MetadataReference.CreateFromFile(typeof(Application.CreateUserCommand).Assembly.Location)],
              options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
          );

        var dtoGenerator = new DtoGenerator();


        var cSharpGeneratorDriverdriver = CSharpGeneratorDriver.Create(dtoGenerator);

        // Act
        var driver = cSharpGeneratorDriverdriver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Debug.Assert(diagnostics.IsEmpty);
        Debug.Assert(outputCompilation.SyntaxTrees.Count() == 5);

        GeneratorDriverRunResult runResult = driver.GetRunResult();

        Debug.Assert(runResult.GeneratedTrees.Length == 4);
        Debug.Assert(runResult.Diagnostics.IsEmpty);

        GeneratorRunResult generatorResult = runResult.Results[0];
        var source = generatorResult.GeneratedSources[0].SourceText.ToString();
        Debug.Assert(generatorResult.Generator == dtoGenerator);
        Debug.Assert(generatorResult.Diagnostics.IsEmpty);
        Debug.Assert(generatorResult.GeneratedSources.Length == 4);
        Debug.Assert(generatorResult.Exception is null);
        Assert.Equal(sourceExpected, source);
    }


    [Fact]
    public void Execute_AssemblyApplicationNotExist_DoesNotGenerateDto()
    {
        // Arrange
        var syntaxTree = SyntaxFactory.ParseSyntaxTree(@"
using System;

class CreateUserCommand 
{
    public string UserName { get; set; }
    public string Email { get; set; }
}");


        var compilation = CSharpCompilation.Create("CodeDesignPlus.Net.Dummy",
            syntaxTrees: [syntaxTree],
            references: [],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var dtoGenerator = new DtoGenerator();

        var csharpDriver = CSharpGeneratorDriver.Create(dtoGenerator);

        // Act
        GeneratorDriver driver = csharpDriver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

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
        Assert.Equal(dtoGenerator, generatorResult.Generator);
        Assert.Empty(generatorResult.Diagnostics);
        Assert.Empty(generatorResult.GeneratedSources);
        Assert.Null(generatorResult.Exception);
    }
}
