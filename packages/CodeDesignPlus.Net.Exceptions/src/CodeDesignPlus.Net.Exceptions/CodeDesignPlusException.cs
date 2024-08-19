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