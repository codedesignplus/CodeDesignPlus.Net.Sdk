namespace CodeDesignPlus.Net.RabbitMQ.Attributes;

/// <summary>
/// Attribute to specify the queue name for a class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueueNameAttribute"/> class.
/// </remarks>
/// <param name="entity">The entity associated with the queue.</param>
/// <param name="action">The action associated with the queue.</param>
[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute(string entity, string action) : Attribute
{
    /// <summary>
    /// Gets the action associated with the queue.
    /// </summary>
    public string Action { get; } = action;

    /// <summary>
    /// Gets the entity associated with the queue.
    /// </summary>
    public string Entity { get; } = entity;

    /// <summary>
    /// Gets the full queue name based on the application name, business, and version.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="business">The business context.</param>
    /// <param name="version">The version of the application.</param>
    /// <returns>The full queue name.</returns>
    public string GetQueueName(string appName, string business, string version)
    {
        return $"{business}.{appName}.{version}.{this.Entity}.{this.Action}".ToLower();
    }
}

/// <summary>
/// Attribute to specify the queue name for a class based on a generic entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="QueueNameAttribute{TEntity}"/> class.
/// </remarks>
/// <param name="action">The action associated with the queue.</param>
[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute<TEntity>(string action) : QueueNameAttribute(typeof(TEntity).Name, action) where TEntity : class, IEntity
{
}