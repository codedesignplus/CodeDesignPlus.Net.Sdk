using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using CodeDesignPlus.Net.Event.Bus.Extensions;

namespace CodeDesignPlus.Net.Event.Bus.Test;

/// <summary>
/// Type Invalid to check method <see cref="ServiceCollectionExtensions.GetEventHandlers{TStartupLogic}"/> 
/// in the unit test <see cref="ServiceCollectionExtensionsTest.GetEventHandlers_NotEmpty_EventsHandlers"/>
/// </summary>
public interface IInvalidEventHandler : IEventHandler<UserRegisteredEvent>
{
}

/// <summary>
/// Type Invalid to check method <see cref="ServiceCollectionExtensions.GetEventHandlers{TStartupLogic}"/> 
/// in the unit test <see cref="ServiceCollectionExtensionsTest.GetEventHandlers_NotEmpty_EventsHandlers"/>
/// </summary>
public class InvalidEventHandler : IFake, IEventHandler<UserRegisteredEvent>
{
    /// <summary>
    /// Invocado por el event bus cuando se detecta un evento al que se esta subscrito
    /// </summary>
    /// <param name="data">Información del evento</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
    public Task HandleAsync(UserRegisteredEvent data, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// Fake Interface to check method <see cref="ServiceCollectionExtensions.AddEventsHandlers{TStartupLogic}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
/// in unit test <see cref="ServiceCollectionExtensionsTest.AddEventHandlers_Services_HandlersQueueAndService"/>
/// </summary>
public interface IFake
{

}

/// <summary>
/// Fake event to check method <see cref="ServiceCollectionExtensions.AddEventsHandlers{TStartupLogic}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
/// in unit test <see cref="ServiceCollectionExtensionsTest.AddEventHandlers_Services_HandlersQueueAndService"/>
/// </summary>
public abstract class FakeEvent : EventBase
{

}

/// <summary>
/// Fake event handler to check method <see cref="ServiceCollectionExtensions.AddEventsHandlers{TStartupLogic}(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
/// in unit test <see cref="ServiceCollectionExtensionsTest.AddEventHandlers_Services_HandlersQueueAndService"/>
/// </summary>
public class FakeEventHandler : IEventHandler<FakeEvent>
{
    /// <summary>
    /// Invocado por el event bus cuando se detecta un evento al que se esta subscrito
    /// </summary>
    /// <param name="data">Información del evento</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
    public Task HandleAsync(FakeEvent data, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}