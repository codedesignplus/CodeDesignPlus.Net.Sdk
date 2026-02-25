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
    /// Gets the business context associated with the queue.
    /// </summary>
    public string Business { get; private set; }
    /// <summary>
    /// Gets the action associated with the queue.
    /// </summary>
    public string Action { get; private set; } = action;
    /// <summary>
    /// Gets the entity associated with the queue.
    /// </summary>
    public string Entity { get; private set; } = entity;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueNameAttribute"/> class with the specified business, entity, and action.
    /// </summary>
    /// <param name="business">The business context associated with the queue.</param>
    /// <param name="entity">The entity associated with the queue.</param>
    /// <param name="action">The action associated with the queue.</param>
    public QueueNameAttribute(string business, string entity, string action) : this(entity, action)
    {
        Business = business;
    }

    /// <summary>
    /// Gets the full queue name based on the application name, business, and version.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="business">The business context.</param>
    /// <param name="version">The version of the application.</param>
    /// <returns>The full queue name.</returns>
    public string GetQueueName(string appName, string business, string version)
    {
        return $"{this.Business ?? business}.{appName}.{version}.{this.Entity}.{this.Action}".ToLower();
    }
}

/// <summary>
/// Attribute to specify the queue name for a class based on a generic entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute<TEntity> : QueueNameAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueNameAttribute{TEntity}"/> class with the specified action.
    /// </summary>
    /// <param name="action">The action associated with the queue.</param>
    public QueueNameAttribute(string action) : base(typeof(TEntity).Name, action)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueNameAttribute{TEntity}"/> class with the specified business and action.
    /// </summary>
    /// <param name="business">The business context associated with the queue.</param>
    /// <param name="action">The action associated with the queue.</param>
    public QueueNameAttribute(string business, string action) : base(business, typeof(TEntity).Name, action)
    {

    }
}