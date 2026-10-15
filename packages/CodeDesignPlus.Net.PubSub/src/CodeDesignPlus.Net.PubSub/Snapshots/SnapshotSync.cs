using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Lo que la copia necesita saber de su gemelo de foto.
/// </summary>
internal static class SnapshotSync
{
    /// <summary>
    /// Lee la foto que nombra un gemelo y exige que diga a quién pedírsela.
    /// </summary>
    /// <param name="snapshotType">El gemelo local del evento de foto.</param>
    /// <returns>La clave de la foto, con la aplicación dueña.</returns>
    /// <exception cref="PubSubException">El tipo no es un evento de foto con clave, o su clave no nombra la aplicación dueña.</exception>
    public static SnapshotKey KeyOf(Type snapshotType)
    {
        SnapshotKey key;

        try
        {
            key = SnapshotKey.Of(snapshotType);
        }
        catch (ArgumentException ex)
        {
            throw new PubSubException(ex.Message, ex);
        }

        // Sin la aplicación en la clave, la copia no sabe a quién pedirle: un gemelo siempre nombra a su dueño.
        if (string.IsNullOrWhiteSpace(key.Source))
        {
            throw new PubSubException($"The snapshot event {snapshotType.Name} must declare the owner application in its EventKey.");
        }

        return key;
    }
}
