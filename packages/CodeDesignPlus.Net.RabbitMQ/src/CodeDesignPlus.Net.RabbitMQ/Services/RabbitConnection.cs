namespace CodeDesignPlus.Net.RabbitMQ.Services
{
    /// <summary>
    /// Manages the RabbitMQ connection.
    /// </summary>
    public class RabbitConnection : IRabbitConnection, IDisposable
    {
        /// <summary>
        /// Gets the RabbitMQ connection.
        /// </summary>
        public IConnection Connection { get; }

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitConnection"/> class.
        /// </summary>
        /// <param name="options">The RabbitMQ options.</param>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
        /// <exception cref="Exceptions.RabbitMQException">Thrown when the connection to RabbitMQ server fails.</exception>
        public RabbitConnection(IOptions<RabbitMQOptions> options, IOptions<CoreOptions> coreOptions, ILogger<RabbitConnection> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(coreOptions);
            ArgumentNullException.ThrowIfNull(logger);

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };

            var isConnected = false;
            var retryCount = 0;
            var errors = new List<string>();

            while (!isConnected && retryCount < options.Value.MaxRetry)
            {
                try
                {
                    this.Connection = factory.CreateConnection(coreOptions.Value.AppName);
                    isConnected = this.Connection.IsOpen;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    logger.LogError(ex, "Error connecting. Attempt {retryCount} of {MaxRetries}.", retryCount, options.Value.MaxRetry);
                    errors.Add(ex.Message);

                    if (retryCount < options.Value.MaxRetry)
                        Thread.Sleep(options.Value.RetryInterval);
                }
            }

            if (!isConnected)
                throw new Exceptions.RabbitMQException("Failed to connect to the RabbitMQ server", errors);

            logger.LogInformation("RabbitMQ Connection established successfully.");
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