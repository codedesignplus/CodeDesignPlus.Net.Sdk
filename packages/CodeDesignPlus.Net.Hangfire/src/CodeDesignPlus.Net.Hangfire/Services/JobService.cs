namespace CodeDesignPlus.Net.Hangfire.Services;

/// <summary>
/// Implementación de <see cref="IJobService"/> que delega las operaciones
/// sobre jobs al cliente de Hangfire (<see cref="IBackgroundJobClient"/>) y
/// al gestor de jobs recurrentes (<see cref="IRecurringJobManager"/>).
/// </summary>
public class JobService : IJobService
{
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly IRecurringJobManager recurringJobManager;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="JobService"/>.
    /// </summary>
    /// <param name="backgroundJobClient">Cliente de Hangfire para jobs en segundo plano.</param>
    /// <param name="recurringJobManager">Gestor de jobs recurrentes de Hangfire.</param>
    public JobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        this.backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
        this.recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
    }

    /// <summary>
    /// Encola un job para ejecución inmediata en segundo plano.
    /// </summary>
    /// <typeparam name="T">Tipo del job a encolar.</typeparam>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <returns>Identificador del job encolado.</returns>
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    /// <summary>
    /// Programa un job para ejecutarse después de un delay.
    /// </summary>
    /// <typeparam name="T">Tipo del job a programar.</typeparam>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <param name="delay">Tiempo de espera antes de la ejecución.</param>
    /// <returns>Identificador del job programado.</returns>
    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return backgroundJobClient.Schedule(methodCall, delay);
    }

    /// <summary>
    /// Agrega o actualiza un job recurrente con su expresión cron.
    /// </summary>
    /// <typeparam name="T">Tipo del job recurrente.</typeparam>
    /// <param name="recurringJobId">Identificador único del job recurrente.</param>
    /// <param name="methodCall">Expresión lambda que representa la llamada al método del job.</param>
    /// <param name="cronExpression">Expresión cron que define la frecuencia de ejecución.</param>
    /// <param name="timezone">Timezone IANA para la ejecución. Por defecto: UTC.</param>
    public void AddOrUpdateRecurring<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression, string? timezone = null)
    {
        var tz = string.IsNullOrEmpty(timezone)
            ? TimeZoneInfo.Utc
            : TimeZoneInfo.FindSystemTimeZoneById(timezone);

        recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions
        {
            TimeZone = tz
        });
    }

    /// <summary>
    /// Elimina un job recurrente por su ID.
    /// </summary>
    /// <param name="recurringJobId">Identificador del job recurrente a eliminar.</param>
    public void RemoveRecurring(string recurringJobId)
    {
        recurringJobManager.RemoveIfExists(recurringJobId);
    }
}
