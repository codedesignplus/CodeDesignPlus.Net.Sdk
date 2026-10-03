using System.Security.Cryptography;
using System.Text;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Deriva el identificador de un aviso a partir del hecho que lo origina.
/// </summary>
/// <remarks>
/// Un consumidor que avisa desde un evento recibe el mismo evento otra vez si el bus lo reentrega. Con un id nuevo
/// en cada llamada, la bandeja guarda un segundo aviso igual; con este, la reentrega llega con el mismo id y la
/// bandeja la reconoce. El resultado es un UUID version 5 (por nombre, SHA-1): el mismo par de entradas da siempre el
/// mismo id, en cualquier proceso y en cualquier maquina.
/// </remarks>
public static class NotificationIds
{
    /// <summary>
    /// El id del aviso de tipo <paramref name="kind"/> que nace de <paramref name="source"/>.
    /// </summary>
    /// <param name="source">El identificador del hecho de origen; normalmente el <c>EventId</c> del evento de dominio.</param>
    /// <param name="kind">El tipo de aviso. Distingue dos avisos distintos que nazcan del mismo evento.</param>
    /// <returns>Un identificador estable para ese par.</returns>
    public static Guid For(Guid source, string kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);

        var name = Encoding.UTF8.GetBytes(kind);
        var input = new byte[16 + name.Length];
        source.ToByteArray(bigEndian: true).CopyTo(input, 0);
        name.CopyTo(input, 16);

        var hash = SHA1.HashData(input);
        var bytes = hash.AsSpan(0, 16).ToArray();

        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes, bigEndian: true);
    }
}
