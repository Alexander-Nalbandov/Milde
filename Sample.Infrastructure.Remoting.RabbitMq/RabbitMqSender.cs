using RabbitMQ.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.RabbitMq
{
    internal class RabbitMqSender<TMessage> : ISender<TMessage> where TMessage : IRemoteMessage
    {
        private readonly IModel _channel;
        private readonly string _exchange;
        private readonly ISerializer _serializer;

        public RabbitMqSender(IModel channel, string exchange, ISerializer serializer)
        {
            this._channel = channel;
            this._exchange = exchange;
            this._serializer = serializer;
        }

        public void Send(TMessage message)
        {
            var body = this._serializer.Serialize(message);
            var props = this._channel.CreateBasicProperties();
            props.SetFrom(message.Headers);

            this._channel.BasicPublish(
                exchange: this._exchange,
                routingKey: message.Headers.RoutingKey,
                basicProperties: props,
                body: body
            );
        }
    }
}