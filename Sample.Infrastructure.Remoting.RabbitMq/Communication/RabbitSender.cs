using Autofac;
using RabbitMQ.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitSender<TInterface, TMessage> : ISender<TInterface, TMessage>, IStartable where TMessage : IRemoteMessage
    {
        private IModel _channel;
        private readonly string _exchange;
        private readonly ISerializer _serializer;
        private readonly RabbitConnectionFactory _factory;

        public RabbitSender(RabbitConnectionFactory connectionFactory, ISerializer serializer, string exchange)
        {
            _factory = connectionFactory;
            this._exchange = exchange;
            this._serializer = serializer;
        }

        public void Send(TMessage message)
        {
            var body = this._serializer.Serialize(message);
            var props = this._channel.CreateBasicProperties();
            props.SetFrom(message.Headers);

            IModelExensions.BasicPublish(this._channel, exchange: this._exchange,
                routingKey: message.Headers.RoutingKey,
                basicProperties: props,
                body: body
            );
        }

        public void Start()
        {
            this._channel = _factory.Connect();
            this._channel.ExchangeDeclare(_exchange, "topic", durable: true);
        }
    }
}