using CodeDesignPlus.Net.Security.Abstractions.Models;
using NodaTime;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class LicenseTest
{
    [Fact]
    public void License_Id_Should_Be_Guid()
    {
        // Arrange
        var license = new License();

        // Act
        license.Id = Guid.NewGuid();

        // Assert
        Assert.IsType<Guid>(license.Id);
    }

    [Fact]
    public void License_Name_Should_Be_String()
    {
        // Arrange
        var license = new License();

        // Act
        license.Name = "Test License";

        // Assert
        Assert.IsType<string>(license.Name);
    }

    [Fact]
    public void License_StartDate_Should_Be_Instant()
    {
        // Arrange
        var license = new License();
        var startDate = Instant.FromDateTimeUtc(DateTime.UtcNow);

        // Act
        license.StartDate = startDate;

        // Assert
        Assert.IsType<Instant>(license.StartDate);
    }

    [Fact]
    public void License_ExpirationDate_Should_Be_Instant()
    {
        // Arrange
        var license = new License();
        var expirationDate = Instant.FromDateTimeUtc(DateTime.UtcNow.AddYears(1));

        // Act
        license.ExpirationDate = expirationDate;

        // Assert
        Assert.IsType<Instant>(license.ExpirationDate);
    }

    [Fact]
    public void License_Metadata_Should_Be_Dictionary()
    {
        // Arrange
        var license = new License();
        var metadata = new Dictionary<string, string>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" }
        };

        // Act
        license.Metadata = metadata;

        // Assert
        Assert.IsType<Dictionary<string, string>>(license.Metadata);
    }
}
