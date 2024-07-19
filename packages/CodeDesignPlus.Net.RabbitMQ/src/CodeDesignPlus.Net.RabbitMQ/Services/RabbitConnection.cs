namespace CodeDesignPlus.Net.RabbitMQ.Services
{
    public class RabbitConnection : IRabbitConnection
    {
        public IConnection Connection { get; }
        private bool disposed = false;

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


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    this.Connection.Dispose();

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RabbitConnection()
        {
            Dispose(false);
        }
    }
}
