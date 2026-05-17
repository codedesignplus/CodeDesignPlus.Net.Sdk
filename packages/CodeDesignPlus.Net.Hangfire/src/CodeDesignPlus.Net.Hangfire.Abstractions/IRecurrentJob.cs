namespace CodeDesignPlus.Net.Hangfire.Abstractions;

/// <summary>
/// Contrato que deben implementar todos los jobs recurrentes (cron-based).
/// Decorar las implementaciones con <see cref="Attributes.RecurringJobOptionsAttribute"/>
/// para que sean auto-descubiertos y registrados por <c>AddHangfire</c>.
/// </summary>
public interface IRecurrentJob
{
    /// <summary>
    /// Ejecuta el job. Es llamado por Hangfire según el cron configurado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación del job.</param>
    Task ExecuteAsync(IJobCancellationToken cancellationToken);
}
