using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.Event.Bus.Services
{

    /// <summary>
    /// Implementación por defecto para el servicio <see cref="IQueueService{TEventHandler, TEvent}"/>
    /// </summary>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public class QueueService<TEventHandler, TEvent> : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        /// <summary>
        /// Contiene los eventos notificados por el Event Bus
        /// </summary>
        private readonly ConcurrentQueue<TEvent> queueEvent = new ConcurrentQueue<TEvent>();

        /// <summary>
        /// Manejador de eventos
        /// </summary>
        private readonly TEventHandler eventHandler;

        /// <summary>
        /// Crea una nueva instancia de <see cref="QueueService{TEvent}"/>
        /// </summary>
        /// <param name="eventHandler">Event Handler</param>
        public QueueService(TEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.
        /// </summary>
        /// <returns>The number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.</returns>
        public int Count => this.queueEvent.Count;

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public bool Any() => this.queueEvent.Any();

        /// <summary>
        /// Agrega un objeto al final de la queue
        /// </summary>
        /// <param name="event">El objeto a agregar al final de la Queu</param>
        /// <exception cref="ArgumentNullException">Se genera cuando <paramref name="event"/> es nulo</exception>
        public void Enqueue(TEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var exist = this.queueEvent.Any(x => x.Equals(@event));

            if (!exist)
                this.queueEvent.Enqueue(@event);
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Return Task that represents an asynchronous operation.</returns>
        public async Task DequeueAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (this.queueEvent.TryDequeue(out TEvent @event))
                        await this.eventHandler.HandleAsync(@event, token);
                    else
                        await Task.Delay(TimeSpan.FromSeconds(5), token);
                } catch(Exception ex) {
                    // TODO: Enqueue @event to queue errors

                    // Reintentos: Si decides implementar una lógica de reintentos para los mensajes que fallan, considera tener un límite en el número de reintentos para evitar un bucle infinito en un mensaje que siempre falla. Podrías usar un mecanismo de cola con soporte para retrasos entre reintentos, o simplemente registrar y descartar el mensaje después de un número específico de fallos.

                    // Sincronización: Si hay múltiples consumidores tratando de desencolar mensajes al mismo tiempo, podrías encontrarte con problemas de concurrencia. Asegúrate de que tu cola (queueEvent) sea segura para operaciones concurrentes, o considera usar un bloqueo alrededor de las operaciones de la cola si es necesario.

                    // Monitorizar: Considera agregar monitoreo o logging para saber cuántos mensajes se están procesando, cuántos errores estás teniendo, cuánto tiempo tarda cada mensaje en ser procesado, etc. Esto puede ser muy útil para diagnosticar problemas o para ajustar el rendimiento en el futuro.

                }
            }
        }
    }
}
