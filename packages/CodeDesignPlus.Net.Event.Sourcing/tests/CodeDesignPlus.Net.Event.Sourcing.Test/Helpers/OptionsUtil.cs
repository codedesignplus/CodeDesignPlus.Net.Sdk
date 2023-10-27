namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public static class OptionsUtil
{
    public static readonly EventSourcingOptions Options = new()
    {
        Enable = true,
        Name = nameof(EventSourcingOptions.Name),
        Email = $"{nameof(EventSourcingOptions.Name)}@codedesignplus.com"
    };
}
