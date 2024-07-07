using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.PubSub.Test;

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
[EventKey<UserEntity>(1, "fake.event")]
public abstract class FakeEvent : DomainEvent
{
    protected FakeEvent(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object>? metadata = null)
        : base(aggregateId, eventId, occurredAt, metadata)
    {
    }
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


[EventKey<UserEntity>(1, "event.failed")]
public class EventFailed : DomainEvent
{
    public EventFailed(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object>? metadata = null)
        : base(aggregateId, eventId, occurredAt, metadata)
    {
    }
}

public class EventFailedHandler : IEventHandler<EventFailed>
{
    /// <summary>
    /// Invocado por el event bus cuando se detecta un evento al que se esta subscrito
    /// </summary>
    /// <param name="data">Información del evento</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
    public Task HandleAsync(EventFailed data, CancellationToken token)
    {
        throw new InvalidOperationException();
    }
}