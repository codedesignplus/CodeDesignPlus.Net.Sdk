namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Despierta al despachador de la bandeja de salida en cuanto este proceso encola algo (pendings/245).
/// </summary>
internal sealed class OutboxSignal : IOutboxSignal, IDisposable
{
    private readonly SemaphoreSlim semaphore = new(0, 1);

    /// <inheritdoc/>
    public void Notify()
    {
        try
        {
            this.semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            // Ya había un aviso pendiente: el despachador va a despertar igual y leerá todo lo encolado.
        }
    }

    /// <inheritdoc/>
    public Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return this.semaphore.WaitAsync(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.semaphore.Dispose();
    }
}
