using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// Las traducciones de los catalogos de error, leidas de los ficheros <c>errors.&lt;idioma&gt;.json</c>
/// que cada ensamblado lleva embebidos.
/// </summary>
/// <remarks>
/// El ingles esta aqui como un idioma mas, en su <c>errors.en.json</c>. Es obligatorio —es lo que se
/// devuelve cuando no hay traduccion— pero es tambien un idioma que se puede <b>pedir</b>: si viviera en el
/// C# no figuraria entre los disponibles y un <c>Accept-Language: en-US,en;q=0.9,es;q=0.8</c> acabaria
/// respondiendo en espanol. Anadir un idioma es anadir un fichero, sin tocar C#.
/// <para>
/// Es estatico y sin inyeccion porque un <see cref="Error"/> se declara en un campo estatico del dominio,
/// donde no hay contenedor al que pedirle nada. El coste es una sola carga al arrancar: unos pocos KiB
/// por idioma y microservicio.
/// </para>
/// <para>
/// La clave es <b>solo el codigo</b>, sin capa ni microservicio, y basta: dentro de un proceso solo estan
/// cargados los ensamblados de un microservicio, y sus tres catalogos no comparten codigos —el dominio usa
/// 1xx, la aplicacion 2xx y la infraestructura 3xx—. Que eso se cumpla lo comprueba el arranque.
/// </para>
/// </remarks>
public static class ErrorCatalog
{
    private const string Prefix = "errors.";
    private const string Extension = ".json";

    /// <summary>
    /// El idioma obligatorio y ultimo recurso. Todo codigo tiene que tener entrada aqui.
    /// </summary>
    public const string English = "en";

    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> Catalogs =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Lock Gate = new();

    private static int assembliesLoaded;

    /// <summary>
    /// Los idiomas para los que hay traduccion, incluido el ingles.
    /// </summary>
    public static IReadOnlyCollection<string> Languages
    {
        get
        {
            EnsureLoaded();
            return [.. Catalogs.Keys];
        }
    }

    /// <summary>
    /// Busca la plantilla de un codigo en el idioma pedido.
    /// </summary>
    /// <remarks>
    /// Prueba primero el idioma completo —<c>es-CO</c>— y luego el neutro —<c>es</c>—. Si no hay ninguno
    /// devuelve <c>null</c> y quien llama vuelve a preguntar por <see cref="English"/>.
    /// </remarks>
    /// <param name="code">El codigo del error.</param>
    /// <param name="language">El codigo de idioma pedido; <c>null</c> equivale a no pedir ninguno.</param>
    /// <returns>La plantilla traducida, o <c>null</c> si no hay.</returns>
    public static string? Find(string code, string? language)
    {
        if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(code))
            return null;

        EnsureLoaded();

        if (Catalogs.TryGetValue(language, out var exact) && exact.TryGetValue(code, out var message))
            return message;

        var dash = language.IndexOf('-');

        if (dash > 0
            && Catalogs.TryGetValue(language[..dash], out var neutral)
            && neutral.TryGetValue(code, out message))
            return message;

        return null;
    }

    /// <summary>
    /// Cuantos codigos hay traducidos en un idioma. Sirve para informar de la cobertura, no para decidir nada.
    /// </summary>
    /// <param name="language">El idioma, por ejemplo <c>es</c>.</param>
    public static int Count(string language)
    {
        EnsureLoaded();

        return Catalogs.TryGetValue(language, out var catalog) ? catalog.Count : 0;
    }

    /// <summary>
    /// Los codigos traducidos en un idioma, para comprobar al arrancar que ninguno se quedo huerfano.
    /// </summary>
    /// <param name="language">El idioma, por ejemplo <c>es</c>.</param>
    public static IReadOnlyCollection<string> Codes(string language)
    {
        EnsureLoaded();

        return Catalogs.TryGetValue(language, out var catalog) ? [.. catalog.Keys] : [];
    }

    /// <summary>
    /// Vuelve a leer los ficheros embebidos. Solo para pruebas.
    /// </summary>
    public static void Reset()
    {
        lock (Gate)
        {
            Catalogs.Clear();
            assembliesLoaded = 0;
        }
    }

    /// <summary>
    /// Lee los <c>errors.&lt;idioma&gt;.json</c> de los ensamblados indicados.
    /// </summary>
    /// <param name="assemblies">Los ensamblados a inspeccionar.</param>
    public static void Load(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
            LoadFrom(assembly);
    }

    /// <summary>
    /// Carga los ensamblados del dominio de aplicacion la primera vez, y vuelve a mirar si aparecieron mas.
    /// </summary>
    /// <remarks>
    /// En .NET un ensamblado se carga cuando se usa por primera vez, asi que al arrancar puede no estar
    /// todavia el que declara un catalogo. Comparar cuantos hay es una comprobacion de un entero, y esto
    /// solo ocurre cuando ya se va a lanzar una excepcion.
    /// </remarks>
    private static void EnsureLoaded()
    {
        var current = AppDomain.CurrentDomain.GetAssemblies();

        if (current.Length == assembliesLoaded)
            return;

        lock (Gate)
        {
            if (current.Length == assembliesLoaded)
                return;

            foreach (var assembly in current)
                LoadFrom(assembly);

            assembliesLoaded = current.Length;
        }
    }

    private static void LoadFrom(Assembly assembly)
    {
        if (assembly.IsDynamic)
            return;

        string[] resources;

        try
        {
            resources = assembly.GetManifestResourceNames();
        }
        catch (FileNotFoundException)
        {
            // Un ensamblado que no se puede inspeccionar no aporta traducciones y no es motivo para
            // tumbar el microservicio: se queda con el ingles.
            return;
        }

        foreach (var resource in resources)
        {
            var language = GetLanguage(resource);

            if (language is null)
                continue;

            using var stream = assembly.GetManifestResourceStream(resource);

            if (stream is null)
                continue;

            Merge(language, Read(stream, resource));
        }
    }

    /// <summary>
    /// Saca el idioma del nombre del recurso: <c>…Resources.errors.es.json</c> da <c>es</c>.
    /// </summary>
    private static string? GetLanguage(string resource)
    {
        if (!resource.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
            return null;

        var withoutExtension = resource[..^Extension.Length];
        var index = withoutExtension.LastIndexOf(Prefix, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
            return null;

        var language = withoutExtension[(index + Prefix.Length)..];

        return string.IsNullOrWhiteSpace(language) || language.Contains('.') ? null : language;
    }

    private static Dictionary<string, string> Read(Stream stream, string resource)
    {
        var file = JsonSerializer.Deserialize<ErrorCatalogFile>(stream, JsonOptions)
            ?? throw new InvalidOperationException($"The error catalog '{resource}' is empty or is not valid JSON.");

        return file.Errors ?? [];
    }

    private static void Merge(string language, Dictionary<string, string> entries)
    {
        var catalog = Catalogs.GetOrAdd(language, _ => new Dictionary<string, string>(StringComparer.Ordinal));

        foreach (var entry in entries)
            catalog[entry.Key] = entry.Value;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private sealed class ErrorCatalogFile
    {
        public string? Language { get; set; }

        public Dictionary<string, string>? Errors { get; set; }
    }
}
