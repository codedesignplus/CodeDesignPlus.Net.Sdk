using CodeDesignPlus.Net.Criteria.Exceptions;

namespace CodeDesignPlus.Net.Criteria.Test.Exceptions;

public class CriteriaExceptionTest
{
    [Fact]
    public void CriteriaException_DefaultValues_EmptyCriteriaException()
    {
        // Arrange & Act
        var exception = new CriteriaException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Criteria.Exceptions.CriteriaException' was thrown.", exception.Message);
    }

    [Fact]
    public void CriteriaException_IsACodeDesignPlusExceptionOfTheApplicationLayer()
    {
        // Arrange & Act
        var exception = new CriteriaException(CriteriaErrors.UnknownProperty.With("Name"));

        // Assert
        var codeDesignPlusException = Assert.IsAssignableFrom<CodeDesignPlus.Net.Exceptions.CodeDesignPlusException>(exception);
        Assert.Equal(CodeDesignPlus.Net.Exceptions.Layer.Application, codeDesignPlusException.Layer);
    }

    [Fact]
    public void CriteriaException_FromCatalogError_MessageIsTheEnglishTemplateWithItsArguments()
    {
        // Arrange & Act
        var exception = new CriteriaException(CriteriaErrors.UnknownProperty.With("Name"));

        // Assert
        Assert.Equal("The property 'Name' does not exist, so it cannot be used to filter or sort.", exception.Message);
    }

    [Theory]
    [InlineData("es", "La propiedad «Name» no existe, así que no se puede usar para filtrar ni para ordenar.")]
    [InlineData("pt", "A propriedade “Name” não existe, então não pode ser usada para filtrar nem para ordenar.")]
    [InlineData("fr", "La propriété « Name » n'existe pas, elle ne peut donc pas servir à filtrer ni à trier.")]
    public void CriteriaErrors_EveryLanguage_HasTheTranslation(string language, string expected)
    {
        // Arrange
        var error = CriteriaErrors.UnknownProperty.With("Name");

        // Act
        var message = error.GetMessage(language);

        // Assert
        Assert.Equal(expected, message);
    }

    [Fact]
    public void CriteriaErrors_EveryCode_HasAMessageInTheFourLanguages()
    {
        // Arrange
        var codes = typeof(CriteriaErrors).GetFields()
            .Where(field => field.FieldType == typeof(CodeDesignPlus.Net.Exceptions.Error))
            .Select(field => ((CodeDesignPlus.Net.Exceptions.Error)field.GetValue(null)!).Code)
            .ToList();

        // Act
        var missing = new[] { "en", "es", "pt", "fr" }
            .SelectMany(language => codes.Where(code => CodeDesignPlus.Net.Exceptions.ErrorCatalog.Find(code, language) is null).Select(code => $"{language}:{code}"))
            .ToList();

        // Assert
        Assert.Empty(missing);
    }

    [Fact]
    public void CriteriaException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new CriteriaException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Criteria.Exceptions.CriteriaException' was thrown.", exception.Message);
    }

    [Fact]
    public void CriteriaException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new CriteriaException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void CriteriaException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new CriteriaException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void CriteriaException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new CriteriaException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void CriteriaException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new CriteriaException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }
}
