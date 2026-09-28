using CodeDesignPlus.Net.xUnit.Microservice.Validations;
using FluentValidation;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

public class StandardFieldLengthsTest
{
    [Theory]
    [InlineData("Name", FieldLength.Name)]
    [InlineData("HolderName", FieldLength.Name)]
    [InlineData("Contact.Name", FieldLength.Name)]
    [InlineData("Title", FieldLength.Title)]
    [InlineData("Subject", FieldLength.Title)]
    [InlineData("Description", FieldLength.Description)]
    [InlineData("Reason", FieldLength.Description)]
    [InlineData("Notes", FieldLength.Notes)]
    [InlineData("ReviewNotes", FieldLength.Notes)]
    [InlineData("AppealReason", FieldLength.Notes)]
    public void ExpectedLength_StandardFamily_ReturnsItsLength(string propertyName, int expected)
    {
        Assert.Equal(expected, StandardFieldLengths.ExpectedLength(propertyName));
    }

    [Theory]
    [InlineData("Code")]
    [InlineData("Email")]
    [InlineData("Phone")]
    [InlineData("NameNative")]
    [InlineData("TermsOfService")]
    public void ExpectedLength_FieldWithItsOwnFormat_ReturnsNull(string propertyName)
    {
        Assert.Null(StandardFieldLengths.ExpectedLength(propertyName));
    }

    [Fact]
    public void FindViolations_NonStandardLengths_ReportsEachOne()
    {
        var violations = StandardFieldLengths.FindViolations(typeof(StandardFieldLengthsTest).Assembly);

        Assert.Contains("SampleCommand.Name: 128, se espera 100", violations);
        Assert.Contains("SampleCommand.Description: 512, se espera 500", violations);
        Assert.DoesNotContain(violations, x => x.StartsWith("SampleCommand.Title"));
        Assert.DoesNotContain(violations, x => x.StartsWith("SampleCommand.Code"));
    }

    [Fact]
    public void FindViolations_DeclaredException_IsSkipped()
    {
        var violations = StandardFieldLengths.FindViolations(typeof(StandardFieldLengthsTest).Assembly, "SampleCommand.Name");

        Assert.DoesNotContain(violations, x => x.StartsWith("SampleCommand.Name"));
        Assert.Contains("SampleCommand.Description: 512, se espera 500", violations);
    }

    public record SampleCommand(string Name, string Description, string Title, string Code);

    public class SampleValidator : AbstractValidator<SampleCommand>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Description).MaximumLength(512);
            RuleFor(x => x.Title).MaximumLength(FieldLength.Title);
            RuleFor(x => x.Code).MaximumLength(4);
        }
    }
}
