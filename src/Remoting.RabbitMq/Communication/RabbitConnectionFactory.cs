using System;
using Milde.Remoting.Rabbit.Configuration;
using RabbitMQ.Client;

namespace Milde.Remoting.Rabbit.Communication
{
    internal class RabbitConnectionFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitConnectionFactory(RabbitConfiguration config)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(config.Address),
                UserName = config.Username,
                Password = config.Password,
                VirtualHost = config.VirtualHost,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
        }

        public IModel Connect()
        {
            var connection = _connectionFactory.CreateConnection();
            return connection.CreateModel();
        }
    }
}