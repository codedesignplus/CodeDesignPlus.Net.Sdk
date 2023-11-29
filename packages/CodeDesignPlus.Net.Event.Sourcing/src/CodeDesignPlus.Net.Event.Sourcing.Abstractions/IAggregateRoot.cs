namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

public interface IAggregateRoot<TUserKey>
{
    string Category {get; }
    Guid Id { get; set; }
    long Version { get; set; }
    void ApplyChange(IDomainEvent @event, TUserKey idUser);

    void ApplyEvent(IDomainEvent @event, Metadata<TUserKey> metadata);
}