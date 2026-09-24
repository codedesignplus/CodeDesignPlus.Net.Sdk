using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;
using Microsoft.AspNetCore.Builder;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

/// <summary>
/// El arranque se niega cuando el catalogo no es coherente.
/// </summary>
/// <remarks>
/// Un guardarrail no vale porque pase, vale porque se comprueba que falla ante una violacion deliberada.
/// Las clases de abajo son esas violaciones: existen solo para que este fichero las encuentre por reflexion,
/// igual que las encontraria en un microservicio.
/// <para>
/// Ojo: la validacion mira <b>todos</b> los catalogos cargados en el proceso, asi que estas clases hacen
/// fallar tambien a cualquier otra prueba que llame a <c>UseCodeErrorsValidation</c>. Por eso no hay una
/// prueba del caso feliz aqui: el caso feliz es todo lo demas del SDK, que arranca.
/// </para>
/// </remarks>
public class CodeErrorsValidationTest
{
    [Fact]
    public void UseCodeErrorsValidation_WithADuplicatedCode_Fails()
    {
        var exception = Assert.Throws<InvalidOperationException>(Validate);

        Assert.Contains("is declared 2 times", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Una constante que sobrevive es un microservicio a medio migrar: sus errores se quedarian en ingles
    /// para siempre y nadie se enteraria.
    /// </summary>
    [Fact]
    public void UseCodeErrorsValidation_WithALegacyConstant_Fails()
    {
        var exception = Assert.Throws<InvalidOperationException>(Validate);

        Assert.Contains("is still a string instead of an Error", exception.Message, StringComparison.Ordinal);
    }

    private static void Validate()
    {
        var app = new Mock<IApplicationBuilder>();

        app.Object.UseCodeErrorsValidation();
    }
}

/// <summary>Dos errores con el mismo codigo: la violacion que la prueba de arriba espera.</summary>
public class ErrorsConCodigoRepetido : IErrorCodes
{
    public static readonly Error Primero = new("8001", "The first one.");

    public static readonly Error Segundo = new("8001", "The second one, with the very same code.");
}

/// <summary>Un catalogo a medio migrar, con una constante todavia dentro.</summary>
public class ErrorsAMedioMigrar : IErrorCodes
{
    public static readonly Error Migrado = new("8002", "This one is already an Error.");

    public const string SinMigrar = "8003 : This one is still a string.";
}
