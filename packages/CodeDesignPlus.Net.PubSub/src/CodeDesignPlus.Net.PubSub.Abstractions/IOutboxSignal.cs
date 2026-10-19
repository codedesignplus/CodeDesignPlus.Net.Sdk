namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// El aviso al despachador de la bandeja de salida de que hay algo nuevo que enviar (pendings/245).
/// </summary>
public interface IOutboxSignal
{
    /// <summary>
    /// Avisa que se encoló algo. Varios avisos seguidos despiertan al despachador una sola vez.
    /// </summary>
    void Notify();

    /// <summary>
    /// Espera un aviso o a que pase el plazo, lo que ocurra antes.
    /// </summary>
    /// <param name="timeout">El plazo máximo de espera.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken);
}
