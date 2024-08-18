namespace CodeDesignPlus.Net.Core.Abstractions.Attributes;

/// <summary>
/// Specifies the key information for an event.
/// </summary>
/// <remarks>
/// This attribute can be applied to any element.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="EventKeyAttribute"/> class with the specified entity, version, and event.
/// </remarks>
/// <param name="entity">The entity associated with the event.</param>
/// <param name="version">The version of the event.</param>
/// <param name="event">The name of the event.</param>
[AttributeUsage(AttributeTargets.All)]
public class EventKeyAttribute(string entity, ushort version, string @event) : Attribute
{
    /// <summary>
    /// Gets the version of the event.
    /// </summary>
    public string Version { get; } = $"v{version}";

    /// <summary>
    /// Gets the entity associated with the event.
    /// </summary>
    public string Entity { get; } = entity;

    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    public string Event { get; } = @event;
}

[AttributeUsage(AttributeTargets.Class)]
public class EventKeyAttribute<TAggregate>(ushort version, string @event)
    : EventKeyAttribute(typeof(TAggregate).Name, version, @event) where TAggregate : IEntityBase;