using Autofac;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Configuration;
using Sample.Infrastructure.Remoting.Registration;
using Sample.Infrastructure.Remoting.Service;

namespace Sample.Infrastructure.Remoting.Rabbit.Registration
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        private readonly ContainerBuilder _builder;

        internal ServiceConfigurator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        public void RegisterProxy<TInterface>()
        {
            _builder.RegisterType<RabbitListener<TInterface, RemoteResponse>>()
                .WithParameter("exchange", RabbitConfiguration.ResponseExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<RabbitSender<TInterface, RemoteRequest>>()
                .WithParameter("exchange", RabbitConfiguration.RequestExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<RemoteProcedureExecutor<TInterface>>()
                .AsSelf().SingleInstance();
            _builder.Register(cc =>
                    ServiceProxyFactory.Create<TInterface>(cc.Resolve<RemoteProcedureExecutor<TInterface>>(), cc.Resolve<ResponseConverter>()))
                .AsImplementedInterfaces().SingleInstance();
        }

        public void RegisterService<TImplementation, TInterface>() where TImplementation : TInterface
        {
            _builder.RegisterType<RabbitListener<TInterface, RemoteRequest>>()
                .WithParameter("exchange", RabbitConfiguration.RequestExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<RabbitSender<TInterface, RemoteResponse>>()
                .WithParameter("exchange", RabbitConfiguration.ResponseExchange)
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<TImplementation>()
                .AsSelf().SingleInstance(); //TODO should this be instance per owned lifetime scope?
            _builder.RegisterType<MessageHandler<TImplementation, TInterface>>()
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}