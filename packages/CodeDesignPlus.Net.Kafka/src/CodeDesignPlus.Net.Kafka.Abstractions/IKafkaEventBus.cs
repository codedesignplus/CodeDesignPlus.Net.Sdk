using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Abstractions;

/// <summary>
/// Represents an interface for a Kafka Event Bus, extending the generic PubSub interface.
/// </summary>
/// <remarks>
/// This interface provides a contract for interacting with a Kafka-based event bus for publishing and subscribing to events.
/// Implementations of this interface should handle the underlying details of communication with Kafka brokers.
/// </remarks>
public interface IKafkaEventBus: IMessage
{
    
}
