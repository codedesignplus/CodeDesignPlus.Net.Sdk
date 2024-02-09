namespace CodeDesignPlus.Net.Core.Abstractions.Attributes;

/// <summary>
/// Attribute to define the key of the event.
/// </summary>
/// <param name="key">The key of the event.</param>
[AttributeUsage(AttributeTargets.All)]
public class KeyAttribute(string key) : Attribute
{
    /// <summary>
    /// Get the key of the event.
    /// </summary>
    public string Key { get; } = key;
}
