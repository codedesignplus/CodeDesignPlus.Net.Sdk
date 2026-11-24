using System.Reflection;

namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// La revisión de la foto que espera una copia: subirla en el gemelo hace que la copia vuelva a pedir la foto completa
/// aunque ya figure como sincronizada.
/// </summary>
/// <remarks>
/// Una fuente sincronizada no se vuelve a pedir. Si el gemelo crece (el dueño empieza a mandar un campo que la copia
/// necesita) o la copia cambia cómo aplica la foto, lo que ya tiene se quedó con la forma vieja y ningún evento en vivo la
/// corrige mientras el agregado no cambie. Subir la revisión es la forma de decirlo en el código: al arrancar, la copia ve
/// que su marca es de una revisión anterior y pide la foto otra vez. Sin el atributo la revisión es 1, la misma que se
/// supone en una marca que no la guarda. Publicar primero el dueño, para que la foto nueva ya traiga lo que se espera.
/// </remarks>
/// <param name="revision">La revisión, desde 1.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SnapshotRevisionAttribute(int revision) : Attribute
{
    /// <summary>
    /// La revisión que tiene una foto sin el atributo, y una marca que no la guarda.
    /// </summary>
    public const int Initial = 1;

    /// <summary>
    /// La revisión de la foto que espera la copia.
    /// </summary>
    public int Revision { get; } = revision >= Initial ? revision : throw new ArgumentOutOfRangeException(nameof(revision), revision, $"The snapshot revision must be {Initial} or greater.");

    /// <summary>
    /// Lee la revisión que declara un gemelo de foto.
    /// </summary>
    /// <param name="snapshotType">El gemelo local del evento de foto.</param>
    /// <returns>La revisión del atributo, o <see cref="Initial"/> si no lo tiene.</returns>
    public static int Of(Type snapshotType)
    {
        ArgumentNullException.ThrowIfNull(snapshotType);

        return snapshotType.GetCustomAttribute<SnapshotRevisionAttribute>()?.Revision ?? Initial;
    }
}
