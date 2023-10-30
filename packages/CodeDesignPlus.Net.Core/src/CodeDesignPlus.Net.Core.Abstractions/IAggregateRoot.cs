namespace CodeDesignPlus.Net.Core.Abstractions;

public interface IAggregateRoot<TUserKey>
{
    Guid Id { get; set; }
    long Version { get; set; }
    void ApplyChange(IDomainEvent @event, TUserKey idUser);

    void ApplyEvent(IDomainEvent @event, Metadata<TUserKey> metadata);
}