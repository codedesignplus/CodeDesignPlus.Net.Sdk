namespace CodeDesignPlus.Net.Hangfire.Abstractions.Options;

/// <summary>
/// Tipo de storage que Hangfire usará como backend de persistencia.
/// </summary>
public enum HangfireStorageType
{
    /// <summary>
    /// Usa Redis como storage vía <c>IRedisFactory</c> del SDK CodeDesignPlus.
    /// </summary>
    Redis,

    /// <summary>
    /// Usa MongoDB como storage vía la conexión del SDK CodeDesignPlus.
    /// </summary>
    Mongo
}

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
    /// Tipo de storage a utilizar. Por defecto: Redis.
    /// </summary>
    public HangfireStorageType StorageType { get; set; } = HangfireStorageType.Redis;

    /// <summary>
    /// Prefijo para las claves del storage. Debe ser único por microservicio.
    /// En Redis: prefijo de clave (ej. "hangfire:ms-invoicing:").
    /// En MongoDB: prefijo de colección (ej. "hangfire.ms-invoicing.").
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

    /// <summary>
    /// Configuración específica para MongoDB storage.
    /// Solo aplica cuando <see cref="StorageType"/> es <see cref="HangfireStorageType.Mongo"/>.
    /// </summary>
    public HangfireMongoOptions Mongo { get; set; } = new();
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

/// <summary>
/// Opciones específicas para el storage de MongoDB en Hangfire.
/// Solo aplica cuando <see cref="HangfireOptions.StorageType"/> es <see cref="HangfireStorageType.Mongo"/>.
/// </summary>
public class HangfireMongoOptions
{
    /// <summary>
    /// Nombre de la base de datos MongoDB donde Hangfire almacena sus colecciones.
    /// Por defecto: "hangfire".
    /// </summary>
    public string DatabaseName { get; set; } = "hangfire";

    /// <summary>
    /// Verifica la conexión a MongoDB al iniciar el servidor.
    /// Por defecto: true.
    /// </summary>
    public bool CheckConnection { get; set; } = true;
}
