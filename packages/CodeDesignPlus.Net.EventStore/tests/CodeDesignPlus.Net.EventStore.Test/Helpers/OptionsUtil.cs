using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers;

public static class OptionsUtil
{
    public static readonly EventStoreOptions EventStoreOptions = new()
    {
        Servers = new Dictionary<string, Server>()
        {
            {
                EventStoreFactoryConst.Core, new Server()
                {
                    ConnectionString = new Uri("tcp://localhost:1113"),
                    User = "admin",
                    Password = "12345678"
                }
            }
        }
    };

    public static EventStoreOptions GetOptions(string ip, int port)
    {
        return new()
        {
            Servers = new Dictionary<string, Server>()
            {
                {
                    EventStoreFactoryConst.Core, new Server()
                    {
                        ConnectionString = new Uri($"tcp://{ip}:{port}") ,
                        User = "admin",
                        Password = "12345678"
                    }
                }
            }
        };
    }

    public static CoreOptions GetCoreOptions()
    {
        return new()
        {
            AppName = "AppTest",
            Version = "1.0.0",
            Description= "Description Test",
            Business = "Business Test",
            Contact = new Contact()
            {
                Name = "Test",
                Email = "test@test.com"
            }

        };
    }
}
