using CodeDesignPlus.Net.Event.Sourcing.Extensions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Extensions;

public class EventSourcingExtesionsTest
{
    [Fact]
    public void GetEventSourcing_ScanAssemblies_EventSourcingServiceFake()
    {
        var eventSourcing = EventSourcingExtesions.GetEventSourcing();

        Assert.Equal(typeof(EventSourcingServiceFake<>), eventSourcing);
    }
}
