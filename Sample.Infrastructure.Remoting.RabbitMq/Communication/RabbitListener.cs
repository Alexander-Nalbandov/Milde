using System;
using Autofac;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Communication
{
    internal class RabbitListener<TInterface, TMessage> : IListener<TInterface, TMessage>, IStartable where TMessage : IRemoteMessage
    {
        private IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        private string _queue => $"{this._exchange}.{this._serviceName}";

        private readonly string _serviceName;
        private readonly string _exchange;
        private readonly ISerializer _serializer;
        private readonly RabbitConnectionFactory _factory;


        public RabbitListener(RabbitConnectionFactory connectionFactory, ISerializer serializer, string exchange)
        {
            _factory = connectionFactory;
            _serializer = serializer;
            _exchange = exchange;
            _consumer = new EventingBasicConsumer(_channel);
            _serviceName = typeof(TInterface).Name;
        }

        public void StartPolling()
        {
            _channel.BasicConsume(_queue,
                true,
                _consumer
            );
        }

        public void AddHandler(Func<TMessage, bool> handler)
        {
            AddHandler(args => true, (msg, args) => handler(msg));
        }

        public void AddHandler(Func<TMessage, BasicDeliverEventArgs, bool> handler)
        {
            AddHandler(args => true, handler);
        }

        public void AddHandler(Predicate<BasicDeliverEventArgs> predicate,
            Func<TMessage, BasicDeliverEventArgs, bool> handler)
        {
            _consumer.Received += (sender, args) =>
            {
                if (!predicate(args))
                    return;

                var response = _serializer.Deserialize<TMessage>(args.Body);
                if (handler(response, args))
                    _channel.BasicAck(args.DeliveryTag, false);
            };
        }

        public void Start()
        {
            _channel = _factory.Connect();
            this._channel.QueueDeclare(this._queue, durable: true);
            this._channel.QueueBind(this._queue, this._exchange, $"{this._serviceName}.*");
            StartPolling();
        }
    }
}