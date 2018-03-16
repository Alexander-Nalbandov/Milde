using Autofac;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Configuration;
using Sample.Infrastructure.Remoting.Registration;

namespace Sample.Infrastructure.Remoting.Rabbit.Registration
{
    internal class ServiceConfigurator : BaseServiceConfigurator
    {
        internal ServiceConfigurator(ContainerBuilder builder) : base(builder)
        {
        }

        protected override void RegisterRequestListener<TInterface, TMessage>()
        {
            _builder.RegisterType<RabbitListener<TInterface, TMessage>>()
                .WithParameter("exchange", RabbitConfiguration.RequestExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
        }

        protected override void RegisterResponseListener<TInterface, TMessage>()
        {
            _builder.RegisterType<RabbitListener<TInterface, TMessage>>()
                .WithParameter("exchange", RabbitConfiguration.ResponseExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
        }

        protected override void RegisterRequestSender<TInterface, TMessage>()
        {
            _builder.RegisterType<RabbitSender<TInterface, TMessage>>()
                .WithParameter("exchange", RabbitConfiguration.RequestExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
        }

        protected override void RegisterResponseSender<TInterface, TMessage>()
        {
            _builder.RegisterType<RabbitSender<TInterface, TMessage>>()
                .WithParameter("exchange", RabbitConfiguration.ResponseExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}