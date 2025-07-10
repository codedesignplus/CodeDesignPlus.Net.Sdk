using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers;

public static class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "xunit-event-store",
        Description = "The xunit test for the event store library",
        Version = "v1",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };

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
            TypeEntryPoint = "rest",
            Version = "1.0.0",
            Description = "Description Test",
            Business = "Business Test",
            Contact = new Contact()
            {
                Name = "Test",
                Email = "test@test.com"
            }

        };
    }
}
