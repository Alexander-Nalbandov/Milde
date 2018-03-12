using RabbitMQ.Client;
using Sample.Infrastructure.Remoting.Rabbit.Configuration;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitConnectionFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitConnectionFactory(RabbitConfiguration config)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = config.Address,
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