
using CodeDesignPlus.Net.Without.Event.Sourcing.Test;
using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.Event.Sourcing;

namespace CodeDesignPlus.Net.Without.Event.Sourcing;

public class ServiceCollectionExtensionTest
{

    [Fact]
    public void AddEventSourcing_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = OptionsUtil.Options
        });

        var serviceCollection = new ServiceCollection();

        // Assert && Act
        Assert.Throws<EventSourcingNotImplementedException>(() => serviceCollection.AddEventSourcing(configuration));
    }
}
