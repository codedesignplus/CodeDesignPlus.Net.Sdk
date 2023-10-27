using CodeDesignPlus.Event.Bus.Without.EventBusService.Test.Helpers;
using CodeDesignPlus.Net.Event.Bus.Extensions;
using CodeDesignPlus.Net.Event.Bus.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Event.Bus.Without.EventBusService.Test;

/// <summary>
///  Pruebas unitarias a la clase <see cref="EventBusExtensions"/>
/// </summary>
public class EventBusExtensionsTest
{
    /// <summary>
    /// Valida que se genere la excepción cuando no se encuentra un servicio que implemente la interfaz <see cref="IEventBus"/>
    /// </summary>
    [Fact]
    public void AddEventBus_EventNotImplemented_EventNotImplementedException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<EventNotImplementedException>(() => services.AddEventBus(configuration));
    }
}