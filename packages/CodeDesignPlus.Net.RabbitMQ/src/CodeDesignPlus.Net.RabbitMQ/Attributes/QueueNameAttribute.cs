using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.RabbitMQ.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute(string entity, string action) : Attribute
{
    public string Action { get; } = action;

    public string Entity { get; } = entity;

    public string GetQueueName(string appName, string business, string version)
    {
        return $"{appName}.{business}.{version}.{this.Entity}.{this.Action}".ToLower();
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute<TEntity>(string action) : QueueNameAttribute(typeof(TEntity).Name, action) where TEntity : class, IEntity
{
}
