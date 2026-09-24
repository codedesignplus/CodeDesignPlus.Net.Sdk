using System.Globalization;

namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// Un error del catalogo de un microservicio: su codigo y su mensaje.
/// </summary>
/// <remarks>
/// Sustituye a la constante <c>"201 : The user was not found."</c>. Una constante es un solo valor y no
/// puede llevar dos idiomas; este objeto conserva el ingles —que es el idioma obligatorio y el ultimo
/// recurso— y deja que las traducciones vivan en los ficheros <c>errors.&lt;idioma&gt;.json</c> que
/// acompanan a cada catalogo.
/// <para>
/// El ingles se queda aqui, junto al codigo, a proposito: abrir un <c>Errors.cs</c> y entender que
/// significa cada error es lo que se hace todos los dias; anadir un idioma se hace una vez.
/// </para>
/// <para>
/// <b>El idioma no se resuelve al lanzar.</b> El objeto viaja entero dentro de la excepcion y se traduce
/// en el borde, en el middleware. Asi el log queda en un solo idioma —que es donde se lee un fallo de
/// madrugada— y la pantalla sale en el del usuario. Resolver al lanzar mezclaria los logs segun quien
/// estuviera usando la aplicacion en ese momento.
/// </para>
/// </remarks>
public sealed class Error
{
    private static readonly object[] SinArgumentos = [];

    /// <summary>
    /// El codigo, unico dentro del catalogo. Es lo que identifica al error fuera del repositorio.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// El mensaje en ingles. Es obligatorio y es lo que se devuelve cuando no hay traduccion.
    /// </summary>
    public string Fallback { get; }

    /// <summary>
    /// Los valores que rellenan los <c>{0}</c> de la plantilla, si los tiene.
    /// </summary>
    public IReadOnlyList<object?> Arguments { get; }

    /// <summary>
    /// Declara un error del catalogo.
    /// </summary>
    /// <param name="code">El codigo, unico dentro del catalogo.</param>
    /// <param name="fallback">El mensaje en ingles.</param>
    public Error(string code, string fallback) : this(code, fallback, SinArgumentos) { }

    private Error(string code, string fallback, IReadOnlyList<object?> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(fallback);

        this.Code = code.Trim();
        this.Fallback = fallback.Trim();
        this.Arguments = arguments;
    }

    /// <summary>
    /// Devuelve el mismo error con los valores que rellenan su plantilla.
    /// </summary>
    /// <remarks>
    /// No formatea nada todavia: guarda los argumentos para que el formateo ocurra en el borde, contra la
    /// plantilla del idioma que toque. Formatear aqui rellenaria el <c>{0}</c> sobre la plantilla inglesa
    /// y la traduccion llegaria tarde.
    /// </remarks>
    /// <param name="arguments">Los valores, en el orden de los <c>{0}</c>, <c>{1}</c>… de la plantilla.</param>
    /// <returns>Una copia con los argumentos; el error declarado en el catalogo no se toca.</returns>
    public Error With(params object?[] arguments) => new(this.Code, this.Fallback, arguments ?? SinArgumentos);

    /// <summary>
    /// El codigo. Existe con este nombre para que sigan compilando las llamadas que venian de la constante.
    /// </summary>
    public string GetCode() => this.Code;

    /// <summary>
    /// El mensaje en el idioma de la peticion en curso, o en ingles si no hay ninguno.
    /// </summary>
    public string GetMessage() => this.GetMessage(ErrorLanguage.Current);

    /// <summary>
    /// El mensaje en el idioma indicado, cayendo al ingles si no hay traduccion.
    /// </summary>
    /// <param name="language">El codigo de idioma pedido, por ejemplo <c>es</c> o <c>fr-CA</c>.</param>
    /// <remarks>
    /// Recibe una cadena y no una <see cref="CultureInfo"/> porque los entrypoints se compilan con
    /// globalizacion invariante y ahi construir una cultura lanza excepcion. Ver <see cref="ErrorLanguage"/>.
    /// </remarks>
    public string GetMessage(string? language)
    {
        var template = ErrorCatalog.Find(this.Code, language) ?? this.Fallback;

        if (this.Arguments.Count == 0)
            return template;

        return string.Format(CultureInfo.InvariantCulture, template, [.. this.Arguments]);
    }

    /// <summary>
    /// El mensaje en ingles <b>con sus argumentos ya puestos</b>.
    /// </summary>
    /// <remarks>
    /// Es lo que va al <see cref="Exception.Message"/> y por tanto al log. Sin esto el log guardaria la
    /// plantilla cruda —«Code is required for {0}-{1}»— y quien leyera el fallo no sabria de que moneda
    /// hablaba.
    /// </remarks>
    public string ToEnglish()
    {
        if (this.Arguments.Count == 0)
            return this.Fallback;

        return string.Format(CultureInfo.InvariantCulture, this.Fallback, [.. this.Arguments]);
    }

    /// <summary>
    /// La forma antigua, <c>"codigo : mensaje"</c>.
    /// </summary>
    /// <remarks>
    /// Se conserva porque las pruebas de formato de los 37 microservicios comprueban ese patron sobre el
    /// valor del campo. Al mantenerlo, esas pruebas siguen valiendo sin tocarlas.
    /// </remarks>
    public override string ToString() => $"{this.Code} : {this.Fallback}";

    /// <summary>
    /// Construye un error desde la forma antigua <c>"codigo : mensaje"</c>.
    /// </summary>
    /// <remarks>
    /// <b>No hay conversion implicita desde <c>string</c> a proposito.</b> La habria si se quisiera migrar
    /// microservicio a microservicio, pero se decidio migrarlos todos a la vez, y sin ella el compilador
    /// obliga a que todo guard reciba una entrada del catalogo: una cadena suelta —un mensaje escrito a
    /// mano en el sitio— deja de compilar en vez de colarse como un error sin codigo.
    /// <para>
    /// Esto se conserva solo para el barrido de migracion y para leer catalogos ajenos. Parte por el
    /// <b>primer</b> <c>:</c>, no por el ultimo: el <c>GetMessage()</c> de la extension partia por el ultimo
    /// y truncaba cualquier mensaje que llevara dos puntos dentro.
    /// </para>
    /// </remarks>
    /// <param name="legacy">La constante con la forma <c>"201 : The user was not found."</c>.</param>
    public static Error FromString(string legacy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(legacy);

        var separator = legacy.IndexOf(':');

        return separator < 0
            ? new Error(legacy, legacy)
            : new Error(legacy[..separator], legacy[(separator + 1)..]);
    }
}
