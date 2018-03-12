using Autofac;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.RabbitMq.Registration
{
    public class ServiceConfigurator
    {
        private readonly ContainerBuilder _builder;

        internal ServiceConfigurator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void Register<TInterface>()
        {
            _builder.Register(cc =>
                    {
                        var factory = cc.Resolve<RabbitConnectionFactory>();
                        var serializer = cc.Resolve<ISerializer>();
                        var listener = new RabbitListener<TInterface>(factory, serializer, typeof(TInterface).Name);
                        var sender = new RabbitSender<TInterface>(factory, serializer, "RPC");
                        return ServiceProxyFactory.Create<TInterface>(
                            new RemoteProcedureExecutor<TInterface>(cc.Resolve<ResponseAwaitersRegistry<RemoteResponse>>(),
                            sender, listener));
                    }
                )
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}