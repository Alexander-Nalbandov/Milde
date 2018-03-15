using Autofac;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Service;

namespace Sample.Infrastructure.Remoting.Registration
{
    public abstract class BaseServiceConfigurator : IServiceConfigurator
    {
        protected readonly ContainerBuilder _builder;

        protected BaseServiceConfigurator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterProxy<TInterface>()
        {
            RegisterResponseListener<TInterface, RemoteResponse>();
            RegisterRequestSender<TInterface, RemoteRequest>();

            _builder.RegisterType<RemoteProcedureExecutor<TInterface>>()
                .AsSelf().SingleInstance();
            _builder.Register(cc =>
                    ServiceProxyFactory.Create(cc.Resolve<RemoteProcedureExecutor<TInterface>>(),
                        cc.Resolve<ResponseConverter>()))
                .AsImplementedInterfaces().SingleInstance();
        }

        public void RegisterService<TImplementation, TInterface>() where TImplementation : TInterface
        {
            RegisterRequestListener<TInterface, RemoteRequest>();
            RegisterResponseSender<TInterface, RemoteResponse>();
            _builder.RegisterType<TImplementation>()
                .AsSelf().SingleInstance();
            _builder.RegisterType<MessageHandler<TImplementation, TInterface>>()
                .AsImplementedInterfaces().SingleInstance();
        }

        protected abstract void RegisterRequestListener<TInterface, TMessage>() where TMessage : IRemoteMessage;
        protected abstract void RegisterResponseListener<TInterface, TMessage>() where TMessage : IRemoteMessage;
        protected abstract void RegisterRequestSender<TInterface, TMessage>() where TMessage : IRemoteMessage;
        protected abstract void RegisterResponseSender<TInterface, TMessage>() where TMessage : IRemoteMessage;
    }
}