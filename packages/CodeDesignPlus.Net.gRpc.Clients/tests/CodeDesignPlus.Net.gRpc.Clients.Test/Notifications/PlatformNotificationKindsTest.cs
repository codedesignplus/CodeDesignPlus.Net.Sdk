namespace CodeDesignPlus.Net.gRpc.Clients.Test.Notifications;

/// <summary>
/// Pruebas del catalogo de tipos de aviso de la plataforma.
/// </summary>
public class PlatformNotificationKindsTest
{
    /// <summary>
    /// El valor es el que guarda la bandeja y el que el frontend registra en <c>notification-kinds.ts</c>: si cambia,
    /// los avisos guardados dejan de casar con su ficha y los nuevos caen al respaldo, sin un solo error.
    /// </summary>
    public static TheoryData<string, string> Kinds => new()
    {
        { PlatformNotificationKinds.LicenseExpiring, "license.expiring" },
        { PlatformNotificationKinds.LicenseExpired, "license.expired" },
        { PlatformNotificationKinds.LicenseBlocked, "license.blocked" },
        { PlatformNotificationKinds.LicenseOffered, "license.offered" },
        { PlatformNotificationKinds.LicenseOrderNotApplied, "license.order-not-applied" },
    };

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Kind_Value_KeepsTheKeyAlreadyRegistered(string actual, string expected)
    {
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Kinds_AllConstants_AreUnique()
    {
        // Arrange
        var values = typeof(PlatformNotificationKinds)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(field => field.IsLiteral)
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToList();

        // Act
        var distinct = values.Distinct().Count();

        // Assert
        Assert.Equal(values.Count, distinct);
    }
}
