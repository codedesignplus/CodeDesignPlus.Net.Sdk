namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public static class OptionsUtil
{
    public static readonly EventSourcingOptions Options = new()
    {
        MainName = "",
        SnapshotSuffix = ""
    };
}
