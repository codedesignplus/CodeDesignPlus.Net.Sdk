namespace CodeDesignPlus.Net.Hangfire.Abstractions.Attributes;

/// <summary>
/// Decora clases que implementan <see cref="IRecurrentJob"/> con los metadatos
/// necesarios para su registro automático como jobs recurrentes de Hangfire.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RecurringJobOptionsAttribute : Attribute
{
    /// <summary>
    /// Expresión cron que define la frecuencia de ejecución (ej: "0 6 * * *").
    /// </summary>
    public string CronExpression { get; }

    /// <summary>
    /// Identificador único del job recurrente. Si no se especifica,
    /// se usa el nombre de la clase en kebab-case.
    /// </summary>
    public string? JobId { get; }

    /// <summary>
    /// Cola de Hangfire en la que se ejecuta el job. Por defecto: "default".
    /// </summary>
    public string Queue { get; }

    /// <summary>
    /// IANA Timezone para la ejecución. Por defecto: UTC.
    /// </summary>
    public string Timezone { get; }

    /// <summary>
    /// Inicializa una nueva instancia del atributo <see cref="RecurringJobOptionsAttribute"/>.
    /// </summary>
    /// <param name="cronExpression">Expresión cron (ej: "0 6 * * *" = diario 06:00 UTC).</param>
    /// <param name="jobId">ID único del job. Opcional; si se omite se usa el nombre de la clase en kebab-case.</param>
    /// <param name="queue">Cola de ejecución. Por defecto: "default".</param>
    /// <param name="timezone">Timezone IANA. Por defecto: "UTC".</param>
    public RecurringJobOptionsAttribute(
        string cronExpression,
        string? jobId = null,
        string queue = "default",
        string timezone = "UTC")
    {
        CronExpression = cronExpression;
        JobId = jobId;
        Queue = queue;
        Timezone = timezone;
    }
}
