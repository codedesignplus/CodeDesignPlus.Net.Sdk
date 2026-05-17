namespace CodeDesignPlus.Net.Hangfire.Abstractions;

/// <summary>
/// Abstracción sobre Hangfire que permite encolar, programar
/// y gestionar jobs recurrentes sin acoplarse directamente a Hangfire.
/// </summary>
public interface IJobService
{
    /// <summary>
    /// Encola un job para ejecución inmediata en segundo plano.
    /// </summary>
    /// <typeparam name="T">Tipo del job a encolar.</typeparam>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <returns>Identificador del job encolado.</returns>
    string Enqueue<T>(Expression<Action<T>> methodCall);

    /// <summary>
    /// Programa un job para ejecutarse después de un delay.
    /// </summary>
    /// <typeparam name="T">Tipo del job a programar.</typeparam>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <param name="delay">Tiempo de espera antes de la ejecución.</param>
    /// <returns>Identificador del job programado.</returns>
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

    /// <summary>
    /// Agrega o actualiza un job recurrente con su expresión cron.
    /// </summary>
    /// <typeparam name="T">Tipo del job recurrente.</typeparam>
    /// <param name="recurringJobId">Identificador único del job recurrente.</param>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <param name="cronExpression">Expresión cron que define la frecuencia de ejecución.</param>
    /// <param name="timezone">Timezone IANA para la ejecución. Por defecto: UTC.</param>
    void AddOrUpdateRecurring<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression, string? timezone = null);

    /// <summary>
    /// Elimina un job recurrente por su ID.
    /// </summary>
    /// <param name="recurringJobId">Identificador del job recurrente a eliminar.</param>
    void RemoveRecurring(string recurringJobId);
}
