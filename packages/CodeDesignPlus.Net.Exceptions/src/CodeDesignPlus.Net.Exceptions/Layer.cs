namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// Represents the different layers in the application where exceptions can occur.
/// </summary>
public enum Layer
{
    /// <summary>
    /// No specific layer.
    /// </summary>
    None,

    /// <summary>
    /// The domain layer.
    /// </summary>
    Domain,

    /// <summary>
    /// The application layer.
    /// </summary>
    Application,

    /// <summary>
    /// The infrastructure layer.
    /// </summary>
    Infrastructure,

    /// <summary>
    /// The API layer.
    /// </summary>
    Api
}