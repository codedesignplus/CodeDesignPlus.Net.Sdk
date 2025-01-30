using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;

public static class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "xunit-event-store-pub-sub",
        Description = "The xunit test for the event store pub sub library",
        Version = "v1",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };
    
    public static readonly EventStorePubSubOptions Options = new()
    {
        Enabled = true
    };

    public static readonly EventStoreOptions EventStoreOptions = new()
    {
        Servers = new Dictionary<string, Server>()
        {
            { EventStoreFactoryConst.Core, new Server() {  ConnectionString = new Uri("tcp://localhost:1113") } }
        }
    };

}
