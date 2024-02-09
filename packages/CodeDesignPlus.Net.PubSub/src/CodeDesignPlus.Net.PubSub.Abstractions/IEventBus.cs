using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Abstractions
{
    /// <summary>
    /// Interface generica para implementación de bus de eventos
    /// </summary>
    public interface IPubSub
    {
        /// <summary>
        /// Metodo encargado de publicar un evento de integración
        /// </summary>
        /// <param name="event">Información del Evento a publicar</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
        Task PublishAsync(IDomainEvent @event, CancellationToken token);
        /// <summary>
        /// Metodo encargado de escuchar un evento de integración
        /// </summary>
        /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
        /// <typeparam name="TEventHandler">Manejador de eventos de integración (Callback)</typeparam>
        /// <param name="token">Cancellation Token</param>
        /// <returns>System.Threading.Tasks.Task que representa la operación asincrónica</returns>
        Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
            where TEvent : IDomainEvent
            where TEventHandler : IEventHandler<TEvent>;

        /// <summary>
        /// Metodo encargado de cancelar la suscripción de un evento
        /// </summary>
        /// <typeparam name="TEvent">Evento de integración a escuchar</typeparam>
        /// <typeparam name="TEventHandler">Manejador de eventos de integración (Callback)</typeparam>
        Task UnsubscribeAsync<TEvent, TEventHandler>()
            where TEvent : IDomainEvent
            where TEventHandler : IEventHandler<TEvent>;
    }
}
