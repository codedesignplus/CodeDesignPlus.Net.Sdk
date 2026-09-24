namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// Represents a custom exception for CodeDesignPlus applications.
/// </summary>
public class CodeDesignPlusException : Exception
{
    /// <summary>
    /// Gets or sets the layer where the exception occurred.
    /// </summary>
    public Layer Layer { get; set; }

    /// <summary>
    /// Gets or sets the error code associated with the exception.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets the catalog error that caused the exception, when there is one.
    /// </summary>
    /// <remarks>
    /// El objeto viaja entero hasta el borde a proposito: el <see cref="Exception.Message"/> se queda en
    /// ingles —que es lo que se lee en un log— y el middleware traduce este al idioma que pida el cliente.
    /// Resolver el idioma al lanzar mezclaria los logs segun quien estuviera usando la aplicacion.
    /// </remarks>
    public Error? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeDesignPlusException"/> class with the specified layer and error code.
    /// </summary>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="code">The error code associated with the exception.</param>
    public CodeDesignPlusException(Layer layer, string code)
    {
        this.Code = code;
        this.Layer = layer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeDesignPlusException"/> class from a catalog error.
    /// </summary>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The catalog error that caused the exception.</param>
    public CodeDesignPlusException(Layer layer, Error error) : base(error.Fallback)
    {
        this.Code = error.Code;
        this.Layer = layer;
        this.Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeDesignPlusException"/> class from a catalog error and an inner exception.
    /// </summary>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The catalog error that caused the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CodeDesignPlusException(Layer layer, Error error, Exception innerException) : base(error.Fallback, innerException)
    {
        this.Code = error.Code;
        this.Layer = layer;
        this.Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeDesignPlusException"/> class with the specified layer, error code, and error message.
    /// </summary>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="code">The error code associated with the exception.</param>
    /// <param name="message">The message that describes the error.</param>
    public CodeDesignPlusException(Layer layer, string code, string message) : base(message)
    {
        this.Code = code;
        this.Layer = layer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeDesignPlusException"/> class with the specified layer, error code, error message, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="code">The error code associated with the exception.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CodeDesignPlusException(Layer layer, string code, string message, Exception innerException) : base(message, innerException)
    {
        this.Code = code;
        this.Layer = layer;
    }
}