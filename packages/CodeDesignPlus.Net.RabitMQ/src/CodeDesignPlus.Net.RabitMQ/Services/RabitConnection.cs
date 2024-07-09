using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.RabitMQ.Abstractions.Options;
using RabbitMQ.Client;

namespace CodeDesignPlus.Net.RabitMQ.Services
{
    public class RabitConnection : IRabitConnection
    {
        public IConnection Connection { get; }
        private bool disposed = false;

        public RabitConnection(IOptions<RabitMQOptions> options, IOptions<CoreOptions> coreOptions, ILogger<RabitConnection> logger)
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
                throw new Exceptions.RabitMQException("Failed to connect to the RabbitMQ server", errors);
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

        ~RabitConnection()
        {
            Dispose(false);
        }
    }
}
