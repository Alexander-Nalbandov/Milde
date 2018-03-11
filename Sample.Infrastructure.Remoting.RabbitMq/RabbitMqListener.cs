using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.RabbitMq
{
    internal class RabbitMqListener<TMessage> : IListener<TMessage> where TMessage : IRemoteMessage
    {
        private readonly IModel _channel;
        private readonly ISerializer _serializer;
        private readonly EventingBasicConsumer _consumer;

        private readonly string _queue;


        public RabbitMqListener(IModel channel, string queue, ISerializer serializer)
        {
            this._channel = channel;
            this._serializer = serializer;
            this._queue = queue;
            this._consumer = new EventingBasicConsumer(channel);
        }

        public void AddHandler(Func<TMessage, BasicDeliverEventArgs, bool> handler)
        {
            this.AddHandler(args => true, handler);
        }

        public void AddHandler(Predicate<BasicDeliverEventArgs> predicate, Func<TMessage, BasicDeliverEventArgs, bool> handler)
        {
            this._consumer.Received += (sender, args) =>
            {
                if (!predicate(args))
                {
                    return;
                }

                var response = this._serializer.Deserialize<TMessage>(args.Body);
                if (handler(response, args))
                {
                    this._channel.BasicAck(args.DeliveryTag, false);
                }
            };
        }

        public void StartPolling()
        {
            this._channel.BasicConsume(
                queue: _queue,
                noAck: true,
                consumer: this._consumer
            );
        }

        public void AddHandler(Func<TMessage, bool> handler)
        {
            this.AddHandler(args => true, (msg, args) => handler(msg));
        }
    }
}
