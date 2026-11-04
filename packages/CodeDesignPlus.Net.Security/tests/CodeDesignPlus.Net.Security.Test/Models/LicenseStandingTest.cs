using CodeDesignPlus.Net.Security.Abstractions.Models;
using NodaTime;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class LicenseStandingTest
{
    private static readonly Instant Expiration = Instant.FromUtc(2026, 11, 4, 5, 0);

    [Fact]
    public void Evaluate_BeforeExpiration_IsActive()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration - Duration.FromDays(1), 15);

        // Assert
        Assert.Equal(LicenseStatus.Active, standing.Status);
    }

    [Fact]
    public void Evaluate_AtTheExpirationInstant_IsStillActive()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration, 15);

        // Assert
        Assert.Equal(LicenseStatus.Active, standing.Status);
    }

    [Fact]
    public void Evaluate_AfterExpirationWithinGrace_IsGraceAndEndsAfterTheGraceDays()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration + Duration.FromSeconds(1), 15);

        // Assert
        Assert.Equal(LicenseStatus.Grace, standing.Status);
        Assert.Equal(Instant.FromUtc(2026, 11, 19, 5, 0), standing.GraceEndsAt);
    }

    [Fact]
    public void Evaluate_AtTheEndOfGrace_IsStillGrace()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration + Duration.FromDays(15), 15);

        // Assert
        Assert.Equal(LicenseStatus.Grace, standing.Status);
    }

    [Fact]
    public void Evaluate_AfterGrace_IsReadOnly()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration + Duration.FromDays(15) + Duration.FromSeconds(1), 15);

        // Assert
        Assert.Equal(LicenseStatus.ReadOnly, standing.Status);
    }

    [Fact]
    public void Evaluate_WithoutGraceDays_GoesStraightToReadOnly()
    {
        // Act
        var standing = LicenseStanding.Evaluate(Expiration, Expiration + Duration.FromSeconds(1), 0);

        // Assert
        Assert.Equal(LicenseStatus.ReadOnly, standing.Status);
    }

    [Fact]
    public void Evaluate_TenantWithoutLicense_IsActiveWithoutOverflow()
    {
        // Arrange: ms-tenants publica Instant.MaxValue como vencimiento cuando la copropiedad no tiene licencia.
        var now = SystemClock.Instance.GetCurrentInstant();

        // Act
        var standing = LicenseStanding.Evaluate(Instant.MaxValue, now, 15);

        // Assert
        Assert.Equal(LicenseStatus.Active, standing.Status);
        Assert.Equal(Instant.MaxValue, standing.GraceEndsAt);
    }

    [Fact]
    public void Evaluate_NegativeGraceDays_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => LicenseStanding.Evaluate(Expiration, Expiration, -1));
    }
}
