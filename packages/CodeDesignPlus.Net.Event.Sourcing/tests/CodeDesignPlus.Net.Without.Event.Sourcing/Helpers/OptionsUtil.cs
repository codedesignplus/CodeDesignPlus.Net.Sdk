using CodeDesignPlus.Net.Event.Sourcing.Options;

namespace CodeDesignPlus.Net.Without.Event.Sourcing.Test;

public class OptionsUtil
{
    public static readonly EventSourcingOptions Options = new()
    {
        Enable = true,
        Name = nameof(EventSourcingOptions.Name),
        Email = $"{nameof(EventSourcingOptions.Name)}@codedesignplus.com"
    };
}
