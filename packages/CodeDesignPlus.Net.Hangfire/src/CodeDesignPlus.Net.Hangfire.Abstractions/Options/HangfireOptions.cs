namespace CodeDesignPlus.Net.Hangfire.Abstractions.Options;

/// <summary>
/// Opciones de configuración para Hangfire en el ecosistema CodeDesignPlus.
/// Se registran desde la sección "Hangfire" del appsettings.json.
/// </summary>
public class HangfireOptions
{
    /// <summary>
    /// Nombre de la sección en appsettings.json.
    /// </summary>
    public const string Section = "Hangfire";

    /// <summary>
    /// Indica si Hangfire está habilitado. Por defecto: true.
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    /// Prefijo para las claves de Redis. Debe ser único por microservicio.
    /// Ejemplo: "hangfire:ms-invoicing:"
    /// </summary>
    public string Prefix { get; set; } = "hangfire:";

    /// <summary>
    /// Número de workers del servidor Hangfire. Por defecto: 1.
    /// </summary>
    public int WorkerCount { get; set; } = 1;

    /// <summary>
    /// Colas que procesa este servidor. Por defecto: ["default"].
    /// </summary>
    public string[] Queues { get; set; } = ["default"];

    /// <summary>
    /// Configuración del dashboard de Hangfire.
    /// </summary>
    public HangfireDashboardOptions Dashboard { get; set; } = new();
}

/// <summary>
/// Opciones del dashboard de Hangfire.
/// </summary>
public class HangfireDashboardOptions
{
    /// <summary>
    /// Indica si el dashboard está habilitado. Por defecto: false.
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// Usuario para autenticación básica del dashboard.
    /// </summary>
    public string Username { get; set; } = "admin";

    /// <summary>
    /// Contraseña para autenticación básica del dashboard.
    /// </summary>
    public string Password { get; set; } = Guid.NewGuid().ToString();
}
