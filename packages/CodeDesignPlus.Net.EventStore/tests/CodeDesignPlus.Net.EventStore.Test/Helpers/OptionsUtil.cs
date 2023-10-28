namespace CodeDesignPlus.Net.EventStore.Test.Helpers;

public static class OptionsUtil
{
    public static readonly EventStoreOptions EventStoreOptions = new()
    {
        Servers = new Dictionary<string, Server>()
        {
            { EventStoreFactoryConst.Core, new Server() {  ConnectionString = new Uri("tcp://localhost:1113") } }
        }
    };
}
