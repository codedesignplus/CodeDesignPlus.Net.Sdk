using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public static class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "xunit-event-sourcing",
        Description = "The xunit test for the event sourcing library",
        Version = "v1",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };

    public static readonly EventSourcingOptions Options = new()
    {
        MainName = "",
        SnapshotSuffix = ""
    };
}
