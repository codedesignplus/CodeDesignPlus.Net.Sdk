using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.RabbitMQ.Consumer.Sample;

[EventKey<UserEntity>(1, "created", "sample-rabbitmq-producer")]
public class UserCreatedEvent(
    Guid aggregateId,
    string name,
    string email,
    string? password = null,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;
    public string Email { get; } = email;
    public string? Password { get; set; } = password;

    public static UserCreatedEvent Create(Guid aggregateId, string name, string email, string? password = null)
    {
        return new UserCreatedEvent(aggregateId, name, email, password);
    }
}
