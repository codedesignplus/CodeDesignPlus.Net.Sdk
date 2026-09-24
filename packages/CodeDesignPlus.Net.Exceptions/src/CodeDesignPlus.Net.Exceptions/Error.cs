using System.Globalization;

namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// Un error del catalogo de un microservicio: su codigo, y nada mas.
/// </summary>
/// <remarks>
/// Los mensajes —en todos los idiomas, <b>incluido el ingles</b>— viven en los ficheros
/// <c>errors.&lt;idioma&gt;.json</c> que acompanan a cada catalogo. El C# solo declara el codigo y le pone
/// nombre.
/// <para>
/// <b>Por que el ingles tambien es un fichero.</b> Cuando el ingles vivia aqui dentro, no era un idioma que
/// se pudiera pedir: era lo que quedaba al agotar la lista del <c>Accept-Language</c>. Por eso una peticion
/// con <c>en-US,en;q=0.9,es;q=0.8</c> respondia en espanol —el ingles no figuraba entre los disponibles, asi
/// que nunca casaba y el turno pasaba al siguiente idioma—. Con el ingles como un fichero mas, los pesos de
/// la cabecera se respetan como manda HTTP. Y de paso desaparece la doble fuente de verdad: el mismo texto
/// ya no esta a la vez en el C# y en un JSON, donde uno de los dos acabaria quedandose atras.
/// </para>
/// <para>
/// <b>Es un <c>struct</c> y no una clase</b> para que declarar un catalogo no cueste memoria: son unos
/// 2.000 errores en el monorepo y cada uno era un objeto en el monton. Asi ocupan lo mismo que la constante
/// que sustituyen, pero siguen siendo un tipo: una guarda recibe <see cref="Error"/> y un numero suelto o
/// un mensaje escrito a mano no compilan.
/// </para>
/// <para>
/// <b>El idioma no se resuelve al lanzar.</b> El error viaja entero dentro de la excepcion y se traduce en
/// el borde, en el middleware. Asi el log queda en un solo idioma —que es donde se lee un fallo de
/// madrugada— y la pantalla sale en el del usuario. Resolver al lanzar mezclaria los logs segun quien
/// estuviera usando la aplicacion en ese momento.
/// </para>
/// </remarks>
public readonly record struct Error
{
    private static readonly object?[] SinArgumentos = [];

    private readonly string? code;
    private readonly IReadOnlyList<object?>? arguments;

    /// <summary>
    /// El codigo, unico dentro del catalogo. Es lo que identifica al error fuera del repositorio.
    /// </summary>
    public string Code => this.code ?? string.Empty;

    /// <summary>
    /// Los valores que rellenan los <c>{0}</c> de la plantilla, si los tiene.
    /// </summary>
    public IReadOnlyList<object?> Arguments => this.arguments ?? SinArgumentos;

    /// <summary>
    /// Declara un error del catalogo.
    /// </summary>
    /// <param name="code">El codigo, unico dentro del catalogo.</param>
    public Error(string code) : this(code, SinArgumentos) { }

    private Error(string code, IReadOnlyList<object?> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        this.code = code.Trim();
        this.arguments = arguments;
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
    public Error With(params object?[] arguments) => new(this.Code, arguments ?? SinArgumentos);

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
    /// <para>
    /// Si no hay ni siquiera ingles devuelve el codigo pelado. Eso solo pasa si el <c>errors.en.json</c> no
    /// llego al ensamblado, y el arranque lo comprueba antes de que ocurra —ver <c>UseCodeErrorsValidation</c>—:
    /// es preferible un codigo a una cadena vacia, porque al menos dice cual es el error.
    /// </para>
    /// </remarks>
    public string GetMessage(string? language)
    {
        var template = ErrorCatalog.Find(this.Code, language)
            ?? ErrorCatalog.Find(this.Code, ErrorCatalog.English)
            ?? this.Code;

        return this.Format(template);
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
        var template = ErrorCatalog.Find(this.Code, ErrorCatalog.English) ?? this.Code;

        return this.Format(template);
    }

    /// <summary>
    /// La forma <c>"codigo : mensaje en ingles"</c>.
    /// </summary>
    /// <remarks>
    /// Se conserva porque las pruebas de formato de los microservicios comprueban ese patron. El mensaje ya
    /// no sale de un campo sino del catalogo ingles, asi que dice lo mismo que decia antes.
    /// </remarks>
    public override string ToString() => $"{this.Code} : {this.ToEnglish()}";

    private string Format(string template)
    {
        var values = this.Arguments;

        if (values.Count == 0)
            return template;

        return string.Format(CultureInfo.InvariantCulture, template, [.. values]);
    }

    /// <summary>
    /// Construye un error a partir de la forma antigua <c>"codigo : mensaje"</c>, quedandose con el codigo.
    /// </summary>
    /// <remarks>
    /// <b>No hay conversion implicita desde <c>string</c> a proposito.</b> Sin ella el compilador obliga a
    /// que toda guarda reciba una entrada del catalogo: una cadena suelta —un mensaje escrito a mano en el
    /// sitio— deja de compilar en vez de colarse como un error sin codigo.
    /// <para>
    /// Esto queda para leer catalogos ajenos que sigan en el formato viejo. Parte por el <b>primer</b>
    /// <c>:</c>, no por el ultimo, porque hay mensajes que llevan dos puntos dentro.
    /// </para>
    /// </remarks>
    /// <param name="legacy">La constante con la forma <c>"201 : The user was not found."</c>.</param>
    public static Error FromString(string legacy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(legacy);

        var separator = legacy.IndexOf(':');

        return separator < 0 ? new Error(legacy) : new Error(legacy[..separator]);
    }
}
