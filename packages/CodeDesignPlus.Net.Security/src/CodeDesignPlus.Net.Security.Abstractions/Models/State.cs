namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the state information.
/// </summary>
public class State
{
    /// <summary>
    /// Gets or sets the identifier of the state.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the state.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the code of the state.
    /// </summary>
    public string Code { get; set; }
}
