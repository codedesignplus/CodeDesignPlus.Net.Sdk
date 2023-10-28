using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;


public class ClientAssignedToOrderEvent : ThinDomainEventBase<ClientMetadata>
{
    public Guid ClientId { get; }
    public string ClientName { get; }

    public ClientAssignedToOrderEvent(Guid orderId, Guid clientId, string clientName, ClientMetadata metadata)
        : base(orderId, metadata)
    {
        ClientId = clientId;
        ClientName = clientName;
    }
}

public class ClientUpdatedForOrderEvent : ThinDomainEventBase<ClientMetadata>
{
    public Guid ClientId { get; }
    public string NewClientName { get; }

    public ClientUpdatedForOrderEvent(Guid orderId, Guid clientId, string newClientName, ClientMetadata metadata)
        : base(orderId, metadata)
    {
        ClientId = clientId;
        NewClientName = newClientName;
    }
}

public class ClientUnassignedFromOrderEvent : ThinDomainEventBase<ClientMetadata>
{
    public Guid ClientId { get; }

    public ClientUnassignedFromOrderEvent(Guid orderId, Guid clientId, ClientMetadata metadata)
        : base(orderId, metadata)
    {
        ClientId = clientId;
    }
}
