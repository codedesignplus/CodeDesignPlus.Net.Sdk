﻿using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Test;

/// <summary>
/// Implementación del Bus de eventos
/// </summary>
public class PubSubService : IPubSub
{
    /// <summary>
    /// Subscription Manager
    /// </summary>
    public ISubscriptionManager Subscription { get; set; }

    /// <summary>
    /// Crea una nueva instancia de <see cref="PubSubService"/>
    /// </summary>
    public PubSubService(ISubscriptionManager subscriptionManager)
    {
        this.Subscription = subscriptionManager;
    }

    /// <summary>
    /// Metodo encargado de publicar un evento de integración
    /// </summary>
    /// <param name="event">Información del Evento a publicar</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
    public Task PublishAsync(IDomainEvent @event, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Metodo encargado de publicar un evento de integración
    /// </summary>
    /// <param name="event">Información del Evento a publicar</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica, la información determinada por la implementación de la interfaz</returns>
    public Task<TResult> PublishAsync<TResult>(IDomainEvent @event, CancellationToken token)
    {
        return Task.FromResult<TResult>(default!);
    }

    /// <summary>
    /// Metodo encargado de escuchar un evento de integración
    /// </summary>
    /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
    /// <typeparam name="TEventHandler">Manejador de eventos de integración (Callback)</typeparam>
    /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Metodo encargado de cancelar la suscripción de un evento
    /// </summary>
    /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
    /// <typeparam name="TEventHandler">Manejador de eventos de integración (Callback)</typeparam>
    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }
}