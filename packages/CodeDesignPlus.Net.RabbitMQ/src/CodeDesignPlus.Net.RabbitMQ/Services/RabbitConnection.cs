
namespace CodeDesignPlus.Net.RabbitMQ.Services
{
    /// <summary>
    /// Manages the RabbitMQ connection.
    /// </summary>
    /// <param name="logger">The logger for logging messages.</param>
    public class RabbitConnection(ILogger<RabbitConnection> logger) : IRabbitConnection
    {
        /// <summary>
        /// Gets the RabbitMQ connection.
        /// </summary>
        public IConnection Connection { get; private set; }

        private bool disposed = false;

        /// <summary>
        /// Connects to the RabbitMQ server.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        /// <param name="settings">The options for configuring the RabbitMQ connection.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> or <paramref name="appName"/> is null.</exception>
        public async Task ConnectAsync(RabbitMQOptions settings, string appName)
        {
            ArgumentNullException.ThrowIfNull(appName);
            ArgumentNullException.ThrowIfNull(settings);

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password
            };

            var isConnected = false;
            var retryCount = 0;
            var errors = new List<string>();

            while (!isConnected && retryCount < settings.MaxRetry)
            {
                try
                {
                    this.Connection = await factory.CreateConnectionAsync(appName);
                    isConnected = this.Connection.IsOpen;

                    this.Connection.CallbackExceptionAsync += CallbackExceiptionAsync;
                    this.Connection.ConnectionBlockedAsync += ConnectionBlockedAsync;
                    this.Connection.ConnectionRecoveryErrorAsync += ConnectionRecoveryErrorAsync;
                    this.Connection.ConnectionShutdownAsync += ConnectionShutdownAsync;
                    this.Connection.ConnectionUnblockedAsync += ConnectionUnblockedAsync;
                    this.Connection.ConsumerTagChangeAfterRecoveryAsync += ConnectionConsumerTagChangeAfterRecoveryAsync;
                    
                }
                catch (Exception ex)
                {
                    retryCount++;
                    logger.LogError(ex, "Error connecting. Attempt {RetryCount} of {MaxRetries}.", retryCount, settings.MaxRetry);
                    errors.Add(ex.Message);

                    if (retryCount < settings.MaxRetry)
                        Thread.Sleep(settings.RetryInterval);
                }
            }

            if (!isConnected)
                throw new Exceptions.RabbitMQException("Failed to connect to the RabbitMQ server", errors);

            logger.LogInformation("RabbitMQ Connection established successfully.");
        }

        private Task ConnectionConsumerTagChangeAfterRecoveryAsync(object sender, ConsumerTagChangedAfterRecoveryEventArgs @event)
        {
            logger.LogInformation("Consumer tag changed after recovery: {TagBefore} -> {TagAfter}", @event.TagBefore, @event.TagAfter);

            return Task.CompletedTask;
        }

        private async Task ConnectionUnblockedAsync(object sender, AsyncEventArgs @event)
        {
            logger.LogInformation("RabbitMQ connection unblocked.");
            await Task.CompletedTask;
        }

        private async Task ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            logger.LogWarning("RabbitMQ connection shutdown: {Message}", @event.ReplyText);
            await Task.CompletedTask;
        }

        private async Task ConnectionRecoveryErrorAsync(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            logger.LogError(@event.Exception, "RabbitMQ connection recovery error: {Message}", @event.Exception.Message);
            await Task.CompletedTask;
        }

        private async Task ConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs @event)
        {
            logger.LogWarning("RabbitMQ connection blocked: {Message}", @event.Reason);
            await Task.CompletedTask;
        }

        private async Task CallbackExceiptionAsync(object sender, CallbackExceptionEventArgs @event)
        {
            logger.LogError(@event.Exception, "RabbitMQ connection error: {Message}", @event.Exception.Message);

            if (this.Connection.IsOpen)
                return;

            try
            {
                await this.ConnectAsync(new RabbitMQOptions(), "RabbitConnection");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reconnect to RabbitMQ: {Message}", ex.Message);
            }
            finally
            {
                this.Connection.CallbackExceptionAsync -= CallbackExceiptionAsync;
            }

            logger.LogInformation("RabbitMQ connection re-established successfully.");
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="RabbitConnection"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    this.Connection.Dispose();

                disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RabbitConnection"/> class.
        /// </summary>
        ~RabbitConnection()
        {
            Dispose(false);
        }
    }
}