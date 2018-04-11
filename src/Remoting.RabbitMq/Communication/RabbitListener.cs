using System;
using Autofac;
using Milde.Remoting.Contracts;
using Milde.Remoting.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Milde.Remoting.Rabbit.Communication
{
    internal class RabbitListener<TInterface, TMessage> : IListener<TInterface, TMessage>, IStartable
        where TMessage : IRemoteMessage
    {
        private readonly EventingBasicConsumer _consumer;
        private readonly string _exchange;
        private readonly RabbitConnectionFactory _factory;
        private readonly ISerializer _serializer;

        private readonly string _serviceName;
        private IModel _channel;


        public RabbitListener(RabbitConnectionFactory connectionFactory, ISerializer serializer, string exchange)
        {
            _factory = connectionFactory;
            _serializer = serializer;
            _exchange = exchange;
            _consumer = new EventingBasicConsumer(_channel);
            _serviceName = typeof(TInterface).Name;
        }

        private string _queue => $"{_exchange}.{_serviceName}";

        public void StartPolling()
        {
            _channel.BasicConsume(_consumer, _queue);
        }

        public void AddHandler(Func<TMessage, bool> handler)
        {
            AddHandler(args => true, (msg, args) => handler(msg));
        }

        public void Start()
        {
            _channel = _factory.Connect();
            _channel.ExchangeDeclare(_exchange, "topic", true, false);
            _channel.QueueDeclare(_queue, true, autoDelete: false, exclusive: false);
            _channel.QueueBind(_queue, _exchange, $"{_serviceName}.*");
            StartPolling();
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
    }
}