using CodeDesignPlus.Net.EventStore.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;

public static class OptionsUtil
{
    public static readonly EventStorePubSubOptions Options = new()
    {

    };

    public static readonly EventStoreOptions EventStoreOptions = new()
    {
        Servers = new Dictionary<string, Server>()
        {
            { EventStoreFactoryConst.Core, new Server() {  ConnectionString = new Uri("tcp://localhost:1113") } }
        }
    };

}
