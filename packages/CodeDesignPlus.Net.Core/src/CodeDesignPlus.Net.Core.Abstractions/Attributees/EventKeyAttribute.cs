namespace CodeDesignPlus.Net.Core.Abstractions.Attributes;

/// <summary>
/// Attribute to define the key of the event.
/// </summary>
/// <param name="entity">The entity that the event belongs to.</param>
/// <param name="version">The version of the event.</param>
/// <param name="event">The name of the event.</param>
[AttributeUsage(AttributeTargets.All)]
public class EventKeyAttribute(string entity, ushort version, string @event) : Attribute
{
    /// <summary>
    /// Get the version of the event
    /// </summary>
    public string Version { get; } = $"v{version}";
    /// <summary>
    /// Get the key of the event.
    /// </summary>
    public string Entity { get; } = entity;
    /// <summary>
    /// Get the name of the event.
    /// </summary>
    public string Event { get; } = @event;
}

/// <summary>
/// Attribute to define the key of the event.
/// </summary>
/// <param name="version">The version of the event.</param>
/// <param name="event">The name of the event.</param>
[AttributeUsage(AttributeTargets.Class)]
public class EventKeyAttribute<TAggregate>(ushort version, string @event) 
    : EventKeyAttribute(typeof(TAggregate).Name, version, @event) where TAggregate : IEntityBase;