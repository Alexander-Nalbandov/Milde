using System;
using Autofac;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitListener<TInterface> : IListener<TInterface>, IStartable
    {
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        private readonly string _queue;
        private readonly ISerializer _serializer;


        public RabbitListener(RabbitConnectionFactory connectionFactory, ISerializer serializer, string queue)
        {
            _channel = connectionFactory.Connect();
            _serializer = serializer;
            _queue = queue;
            _consumer = new EventingBasicConsumer(_channel);
        }

        public void StartPolling()
        {
            _channel.BasicConsume(_queue,
                true,
                _consumer
            );
        }

        public void AddHandler(Func<RemoteResponse, bool> handler)
        {
            AddHandler(args => true, (msg, args) => handler(msg));
        }

        public void AddHandler(Func<RemoteResponse, BasicDeliverEventArgs, bool> handler)
        {
            AddHandler(args => true, handler);
        }

        public void AddHandler(Predicate<BasicDeliverEventArgs> predicate,
            Func<RemoteResponse, BasicDeliverEventArgs, bool> handler)
        {
            _consumer.Received += (sender, args) =>
            {
                if (!predicate(args))
                    return;

                var response = _serializer.Deserialize<RemoteResponse>(args.Body);
                if (handler(response, args))
                    _channel.BasicAck(args.DeliveryTag, false);
            };
        }

        public void Start()
        {
            this._channel.QueueDeclare(_queue, durable: true);
        }
    }
}