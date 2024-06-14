using CodeDesignPlus.Net.RabitMQ.Abstractions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDesignPlus.Net.RabitMQ.Services
{
    public class RabitConnection: IRabitConnection
    {
        public IConnection Connection { get; }

        public RabitConnection(IOptions<RabitMQOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };

            this.Connection = factory.CreateConnection();
        }
    }
}
