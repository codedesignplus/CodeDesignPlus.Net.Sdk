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
/// <param name="appName">The name of the application that generates the event or the name of the application to listen to the event.</param>
[AttributeUsage(AttributeTargets.All)]
public class EventKeyAttribute(string entity, ushort version, string @event, string? appName = null) : Attribute
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

    /// <summary>
    /// Gets the name of the application that generates the event or the name of the application to listen to the event.
    /// </summary>
    public string? AppName { get; } = appName;
}

/// <summary>
/// Specifies the key information for an event.
/// </summary>
/// <typeparam name="TAggregate">The entity associated with the event.</typeparam>
/// <param name="version">The version of the event.</param>
/// <param name="event">The name of the event.</param>
/// <param name="appName">The name of the application that generates the event or the name of the application to listen to the event.</param>
[AttributeUsage(AttributeTargets.Class)]
public class EventKeyAttribute<TAggregate>(ushort version, string @event, string? appName = null)
    : EventKeyAttribute(typeof(TAggregate).Name, version, @event, appName) where TAggregate : IEntityBase;