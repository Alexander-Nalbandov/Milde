using RabbitMQ.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitSender<TInterface> : ISender<TInterface>
    {
        private readonly IModel _channel;
        private readonly string _exchange;
        private readonly ISerializer _serializer;

        public RabbitSender(RabbitConnectionFactory connectionFactory, ISerializer serializer, string exchange)
        {
            this._channel = connectionFactory.Connect();
            this._exchange = exchange;
            this._serializer = serializer;
        }

        public void Send(RemoteRequest message)
        {
            var body = this._serializer.Serialize(message);
            var props = this._channel.CreateBasicProperties();
            Rabbit.Extensions.SetFrom(props, message.Headers);

            IModelExensions.BasicPublish(this._channel, exchange: this._exchange,
                routingKey: message.Headers.RoutingKey,
                basicProperties: props,
                body: body
            );
        }
    }
}