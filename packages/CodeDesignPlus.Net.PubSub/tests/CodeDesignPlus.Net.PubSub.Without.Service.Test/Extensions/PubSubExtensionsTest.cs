using CodeDesignPlus.PubSub.Without.PubSubService.Test.Helpers;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.PubSub.Without.Service.Test;

/// <summary>
///  Pruebas unitarias a la clase <see cref="PubSubExtensions"/>
/// </summary>
public class PubSubExtensionsTest
{
    /// <summary>
    /// Valida que se genere la excepción cuando no se encuentra un servicio que implemente la interfaz <see cref="IPubSub"/>
    /// </summary>
    [Fact]
    public void AddPubSub_EventNotImplemented_EventNotImplementedException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<EventNotImplementedException>(() => services.AddPubSub(configuration));
    }
}