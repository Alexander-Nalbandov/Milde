using Autofac;
using RabbitMQ.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitSender<TInterface, TMessage> : ISender<TInterface, TMessage>, IStartable
        where TMessage : IRemoteMessage
    {
        private readonly string _exchange;
        private readonly RabbitConnectionFactory _factory;
        private readonly ISerializer _serializer;
        private IModel _channel;

        public RabbitSender(RabbitConnectionFactory connectionFactory, ISerializer serializer, string exchange)
        {
            _factory = connectionFactory;
            _exchange = exchange;
            _serializer = serializer;
        }

        public void Send(TMessage message)
        {
            var body = _serializer.Serialize(message);
            var props = _channel.CreateBasicProperties();
            props.SetFrom(message.Headers);

            _channel.BasicPublish(_exchange,
                message.Headers.RoutingKey,
                props,
                body
            );
        }

        public void Start()
        {
            _channel = _factory.Connect();
            _channel.ExchangeDeclare(_exchange, "topic", true, autoDelete: false);
        }
    }
}