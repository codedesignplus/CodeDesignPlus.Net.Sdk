using M = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class TenantTest
{
    private static Currency CreateCurrency() =>
        Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);

    private static Location CreateLocation() =>
        Location.Create(
            Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "America/New_York", CreateCurrency()),
            State.Create(Guid.NewGuid(), "New York", "NY"),
            City.Create(Guid.NewGuid(), "New York", null),
            Locality.Create(Guid.NewGuid(), "Manhattan"),
            Neighborhood.Create(Guid.NewGuid(), "Midtown"),
            "123 Main St",
            "10001"
        );

    [Fact]
    public void Tenant_Id_Should_Be_Guid()
    {
        // Arrange
        var tenant = new M.Tenant();

        // Act
        tenant.Id = Guid.NewGuid();

        // Assert
        Assert.IsType<Guid>(tenant.Id);
    }

    [Fact]
    public void Tenant_Name_Should_Be_String()
    {
        // Arrange
        var tenant = new M.Tenant();

        // Act
        tenant.Name = "Test Tenant";

        // Assert
        Assert.IsType<string>(tenant.Name);
    }

    [Fact]
    public void Tenant_Domain_Should_Be_Uri()
    {
        // Arrange
        var tenant = new M.Tenant();

        // Act
        tenant.Domain = new Uri("https://example.com");

        // Assert
        Assert.IsType<Uri>(tenant.Domain);
    }

    [Fact]
    public void Tenant_License_Should_Be_License()
    {
        // Arrange
        var tenant = new M.Tenant();
        var license = new M.License();

        // Act
        tenant.License = license;

        // Assert
        Assert.IsType<M.License>(tenant.License);
    }

    [Fact]
    public void Tenant_Location_Should_Be_Location()
    {
        // Arrange
        var tenant = new M.Tenant();
        var location = CreateLocation();

        // Act
        tenant.Location = location;

        // Assert
        Assert.IsType<Location>(tenant.Location);
    }

    [Fact]
    public void Tenant_Metadata_Should_Be_Dictionary()
    {
        // Arrange
        var tenant = new M.Tenant();
        var metadata = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        // Act
        tenant.Metadata = metadata;

        // Assert
        Assert.IsType<Dictionary<string, string>>(tenant.Metadata);
    }
}
